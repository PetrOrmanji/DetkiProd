<template>
  <v-dialog
      :model-value="modelValue"
      @update:model-value="onUpdate"
      fullscreen
  >
    <v-card variant="text" class="menu-card d-flex align-center justify-center">
      <!-- Player container -->
      <div class="player-wrap" ref="wrapEl" style="height: 100vh;width: 100vw;">
        <video
            ref="videoEl"
            class="video-js vjs-default-skin"
            playsinline
            autoplay
            preload="auto"
        ></video>

        <!-- Overlay root that will be moved inside player.el() -->
        <div ref="overlayRoot">
          <!-- Close button (top-right) -->
          <v-btn class="close-btn" variant="plain" :ripple="false" @click="close">
            <img src="/icons/close.svg"/>
          </v-btn>

          <!-- Overlay: PAUSED state -->
          <div v-show="isPaused" class="overlay paused">
            <div class="title-large">
              <div class="title-frame" ref="titleFrameEl">
                <span class="title-text" ref="titleTextEl">{{ meta.title || 'Untitled' }}</span>
              </div>
              <div class="meta-side">
                <div class="meta-row">
                  <span class="meta-label">SOFT</span>
                  <span class="meta-value">{{ meta.tools || '-' }}</span>
                </div>
                <div class="meta-row">
                  <span class="meta-label">YEAR</span>
                  <span class="meta-value">{{ meta.year ?? '-' }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Overlay: PLAYING state -->
          <div v-show="!isPaused" class="overlay playing">
            <div class="playing-left">{{ meta.title || 'Untitled' }}</div>
            <div class="playing-right">
              <div class="meta-row">
                <span class="meta-label">SOFT</span>
                <span class="meta-value">{{ meta.tools || '-' }}</span>
              </div>
              <div class="meta-row">
                <span class="meta-label">YEAR</span>
                <span class="meta-value">{{ meta.year ?? '-' }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import {onMounted, onBeforeUnmount, ref, watch, reactive, nextTick} from 'vue'
import {useHead, useRuntimeConfig} from '#imports'

let muteObserver: MutationObserver | null = null;
import {getProjectById, projects} from '~/data/projects'
const props = defineProps<{
  modelValue: boolean
  projectId?: string | number
  project?: any
}>()
const emit = defineEmits<{ (e: 'update:modelValue', value: boolean): void }>()

const videoEl = ref<HTMLVideoElement | null>(null)
const overlayRoot = ref<HTMLElement | null>(null)
const wrapEl = ref<HTMLElement | null>(null)
const titleFrameEl = ref<HTMLElement | null>(null)
const titleTextEl = ref<HTMLElement | null>(null)
let player: any = null
const isPaused = ref(true)

const meta = reactive<{ title: string; src: string; year?: number | string; tools?: string }>(
    {title: '', src: '', year: undefined, tools: undefined}
)
function setupCustomMuteIcon() {
  // 1) Один раз добавляем CSS-правило, которое отключает псевдоэлемент ::before
  if (!document.getElementById('vjsMuteIconOverride')) {
    const style = document.createElement('style');
    style.id = 'vjsMuteIconOverride';
    style.textContent = `
      .player-wrap .vjs-mute-control .vjs-icon-placeholder::before,
      .player-wrap .vjs-volume-panel-button .vjs-icon-placeholder::before {
        content: "" !important;
      }
    `;
    document.head.appendChild(style);
  }

  const root = player.el() as HTMLElement;

  const findIconEl = (): HTMLElement | null => {
    // покрываем оба варианта разметки
    return (
        root.querySelector('.vjs-volume-panel .vjs-volume-panel-button .vjs-icon-placeholder') as HTMLElement ||
        root.querySelector('.vjs-mute-control .vjs-icon-placeholder') as HTMLElement
    );
  };

  const applyBaseStyles = (el: HTMLElement) => {
    el.style.width = '30px';
    el.style.height = '30px';
    el.style.display = 'block';
    el.style.backgroundRepeat = 'no-repeat';
    el.style.backgroundPosition = 'center';
    el.style.backgroundSize = '30px';
  };

  const updateIcon = () => {
    const el = findIconEl();
    if (!el) return;
    applyBaseStyles(el);
    const muted = player.muted() || player.volume() === 0;
    el.style.backgroundImage = `url(${muted ? '/icons/mute.svg' : '/icons/volume.svg'})`;
  };

  // 2) Обновляем при любых изменениях громкости/мьюта и при готовности
  player.on('volumechange', updateIcon);
  player.on('loadedmetadata', updateIcon);
  player.on('ready', updateIcon);

  // 3) На случай перестройки control-bar — следим мутацией DOM
  const bar = root.querySelector('.vjs-control-bar');
  if (bar) {
    muteObserver?.disconnect();
    muteObserver = new MutationObserver(() => updateIcon());
    muteObserver.observe(bar, { subtree: true, childList: true, attributes: true });
  }

  // Первичный вызов
  updateIcon();
}
// --- Autosize paused title to fill the frame ---
function fitTitleToFrame() {
  if (!isPaused.value) return
  const frame = titleFrameEl.value as HTMLElement | null
  const text = titleTextEl.value as HTMLElement | null
  if (!frame || !text) return

  // If not visible (e.g., dialog closed), skip
  const frameRect = frame.getBoundingClientRect()
  if (frameRect.width === 0 || frameRect.height === 0) return

  const paddingX = 12
  const paddingY = 8
  const maxW = Math.max(0, frame.clientWidth - paddingX)
  const maxH = Math.max(0, frame.clientHeight - paddingY)
  if (maxW === 0 || maxH === 0) return

  // Binary search font-size
  let lo = 4, hi = 512, best = 4
  for (let i = 0; i < 18; i++) {
    const mid = Math.floor((lo + hi) / 2)
    text.style.fontSize = mid + 'px'
    // force reflow and measure
    const rect = text.getBoundingClientRect()
    const w = Math.ceil(rect.width)
    const h = Math.ceil(rect.height)
    if (w <= maxW && h <= maxH) {
      best = mid
      lo = mid + 1
    } else {
      hi = mid - 1
    }
  }
  text.style.fontSize = Math.max(4, best) + 'px'

  // After sizing by height/width constraints, stretch horizontally if there is leftover width
  // without affecting height, to fill the frame fully.
  // Measure resulting width
  const finalRect = text.getBoundingClientRect()
  const wFinal = Math.ceil(finalRect.width)
  if (wFinal > 0) {
    const neededScaleX = maxW / wFinal
    // Only stretch if there is a noticeable gap; clamp to avoid over-distortion
    if (neededScaleX > 1.005) {
      const clamped = Math.min(1.25, neededScaleX)
      text.style.transform = `scaleX(${clamped}) scaleY(1.1)`
    } else {
      text.style.transform = `scaleY(1.1)`
    }
  }
}

function debounce(fn: (...args: any[]) => void, wait = 100) {
  let t: any
  return (...args: any[]) => {
    if (t) clearTimeout(t)
    t = setTimeout(() => fn(...args), wait)
  }
}

const refitDebounced = debounce(() => {
  // Fit after DOM updates
  nextTick(() => fitTitleToFrame())
}, 80)

const onResize = () => refitDebounced()

watch(() => isPaused.value, (paused) => {
  if (paused) nextTick(() => refitDebounced())
})

watch(() => meta.title, () => {
  nextTick(() => refitDebounced())
})

useHead({
  link: [
    {rel: 'stylesheet', href: 'https://cdn.jsdelivr.net/npm/video.js@8.13.0/dist/video-js.min.css'}
  ],
  script: [
    {src: 'https://cdn.jsdelivr.net/npm/video.js@8.13.0/dist/video.min.js', defer: true}
  ]
})

function close() {
  emit('update:modelValue', false)
}

function onUpdate(val: boolean) {
  // When dialog visibility changes
  if (!val) {
    // pause on close
    try {
      player?.pause?.()
    } catch {
    }
  }
  emit('update:modelValue', val)
}

async function ensurePlayer() {
  if (!videoEl.value || player) return
  // @ts-ignore
  const vjs = (globalThis as any).videojs
  if (!vjs) {
    // wait a tick for CDN script to attach
    await new Promise(r => setTimeout(r, 50))
  }
  // @ts-ignore
  const videojsLib = (globalThis as any).videojs
  if (!videojsLib || !videoEl.value) return

  player = videojsLib(videoEl.value, {
    controls: true,
    autoplay: false,
    preload: 'auto',
    fluid: true,
    controlBar: {
      remainingTimeDisplay: false,
      pictureInPictureToggle: false,
      fullscreenToggle: false,
    }
  })

  player.ready(() => {
    if (props.modelValue) {
      setupCustomMuteIcon();
      // Небольшая задержка для корректной инициализации
      setTimeout(() => {
        player.requestFullscreen().catch(console.error)
      }, 100)
    }
  })

  player.on('play', () => {
    isPaused.value = false
  })
  player.on('pause', () => {
    isPaused.value = true
  })
  player.on('ended', () => {
    isPaused.value = true
  })

  // Move overlays inside the player container so they are visible in fullscreen
  try {
    const playerEl: HTMLElement = player.el?.() || player.el_ || player.player_?.el?.() || null
    if (playerEl && overlayRoot.value && overlayRoot.value.parentElement !== playerEl) {
      playerEl.appendChild(overlayRoot.value)
    }
  } catch {
  }

  // Load current source if available
  if (meta.src) {
    loadSource(meta.src)
  }

  // After player initialized and overlays are in place, fit the title
  refitDebounced()
}

function disposePlayer() {
  try {
    // Move overlays back to stable wrapper before disposing the player element
    const playerEl: HTMLElement | null = player?.el?.() || player?.el_ || null
    if (overlayRoot.value && wrapEl.value && overlayRoot.value.parentElement === playerEl) {
      wrapEl.value.appendChild(overlayRoot.value)
    }

    if (player) {
      player.dispose()
      player = null
    }
  } catch {
  }
}

function loadSource(src: string) {
  if (player && src) {
    player.src({src, type: guessType(src)})
  } else if (videoEl.value) {
    // fallback native until player ready
    videoEl.value.src = src
  }
}

function guessType(src: string): string {
  if (src.endsWith('.m3u8')) return 'application/x-mpegURL'
  if (src.endsWith('.mpd')) return 'application/dash+xml'
  if (src.endsWith('.webm')) return 'video/webm'
  return 'video/mp4'
}

async function fetchProjectById(id?: string | number, provided?: any) {
  const fallback = {
    title: projects[0]?.name || 'gilmurt',
    src: projects[0]?.videoUrl || '/gilmurt_cut_for_web.mp4',
    year: projects[0]?.year || '2025',
    tools: projects[0]?.tools || 'Blender, Nuke, Houdini'
  }

  const config = useRuntimeConfig()
  let item = provided || (id ? getProjectById(id) : null)

  try {
    const list: any = await $fetch(config.public.projectsUrl)
    if (Array.isArray(list) && list.length) {
      const targetId = provided?.id ?? id
      item = (targetId ? list.find((i: any) => String(i.id) === String(targetId)) : null) || item || list[0]
    }
  } catch (e) {
    // fallback to static item
  }

  const title = item?.name || item?.title || fallback.title
  const yearFromDate = item?.date ? String(item.date).slice(0, 4) : undefined
  const year = yearFromDate || (item?.year ?? fallback.year)
  const tools = Array.isArray(item?.tools) ? item.tools.join(', ') : (item?.tools || fallback.tools)
  const src = item?.videoUrl || fallback.src

  Object.assign(meta, { title, src, year, tools })
  loadSource(meta.src)
}

watch(() => props.modelValue, async (open) => {
  if (open) {
    await fetchProjectById(props.projectId, props.project)
    await ensurePlayer()
  } else {
    disposePlayer()
  }
})

watch(() => props.projectId, async (id) => {
  if (props.modelValue) {
    await fetchProjectById(id, props.project)
    if (player && meta.src) loadSource(meta.src)
  }
})

watch(() => props.project, async (project) => {
  if (props.modelValue) {
    await fetchProjectById(project?.id, project)
    if (player && meta.src) loadSource(meta.src)
  }
})

onMounted(async () => {
  // Listen for viewport changes to refit title
  if (typeof window !== 'undefined') {
    window.addEventListener('resize', onResize)
  }
  if (props.modelValue) {
    await fetchProjectById(props.projectId)
    await ensurePlayer()
  }
  // Initial fit attempt when mounted
  refitDebounced()
})

onBeforeUnmount(() => {
  if (typeof window !== 'undefined') {
    window.removeEventListener('resize', onResize)
  }
  disposePlayer()
})
</script>

<style scoped>

.video-card {
  position: fixed;
  inset: 0;
  background: black;
}

.player-wrap {
  position: relative;
  width: 100vw;
  height: 100vh;
  overflow: hidden;
}

.player-wrap :deep(.video-js) {
  position: relative;
  width: 100%;
  height: 100%;
}

.player-wrap :deep(video),
.player-wrap :deep(.vjs-tech) {
  width: 100% !important;
  height: 100% !important;
  object-fit: contain;
}

/* Hide default big play */
.player-wrap :deep(.vjs-big-play-button) {
  display: none !important;
}

.overlay {
  position: absolute;
  inset: 0;
  color: white;
  pointer-events: none;
  font-family: Georgia, serif;
}

/* PAUSED: big centered title occupying ~80% width, with SOFT/YEAR bottom-right of the title */
.overlay.paused {
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: left;
  padding: 4vw;
}

.title-large {
  position: relative;
  max-width: 80vw;
  width: 80%;
  font-family: Georgia, serif;
}

.title-frame {
  position: relative;
  width: 100%;
  aspect-ratio: 16/3;
  border: 2px solid rgba(255, 255, 255, .7);
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
}

.title-text {
  font-family: Georgia, serif;
  display: inline-block;
  white-space: nowrap;
  letter-spacing: 0.2em;
  transform: scaleY(1.4);
  transform-origin: center center;
  will-change: font-size, transform;
}

.title-large .meta-side {
  font-family: Georgia, serif;
  position: static;
  display: grid;
  grid-template-columns: auto auto; /* col1: label, col2: value */
  justify-content: end; /* stick the grid to the right edge */
  column-gap: 0.6em;
  row-gap: 0.35em; /* equal spacing between SOFT and YEAR */
  margin-top: 0.6em; /* push meta to a new line under the title */
  font-size: clamp(12px, 2.2vw, 24px); /* smaller than the main title */
  line-height: 1.2;
}

.meta-row {
  display: contents; /* let children participate directly in CSS grid */
  white-space: nowrap;
  font-family: none;
}

.meta-label {
  opacity: 0.85;
  font-size: 0.9em;
  justify-self: end; /* right column alignment for label column */
  text-align: right;
}

.meta-value {
  opacity: 0.95;
  justify-self: start; /* left alignment for value column */
  text-align: left;
}

/* Right-align YEAR value on the right edge */
.title-large .meta-side .meta-row:nth-of-type(2) .meta-value,
.playing-right .meta-row:nth-of-type(2) .meta-value {
  justify-self: end;
  text-align: right;
}

/* PLAYING: bottom-left title small, bottom-right SOFT/YEAR */
.overlay.playing {
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  padding: 2vh 3vw;
}

.playing-left {
  border: 1.5px solid white;
  font-size: clamp(14px, 2.2vw, 28px);
  letter-spacing: 0.2em;
  line-height: 1.1;
  transform: scaleY(1.4);
  max-width: 50vw;
  margin-bottom: 1em;
}

.playing-right {
  display: grid;
  grid-template-columns: auto auto; /* col1: label, col2: value */
  justify-content: end; /* pack grid to the right */
  column-gap: 0.6em;
  row-gap: 0.3em; /* equal spacing */
  font-size: clamp(14px, 2.2vw, 20px);
  margin-bottom: 1em;
}


.close-btn {
  position: absolute;
  top: 36px;
  right: 36px;
  z-index: 2;
  pointer-events: auto;
}

.player-wrap :deep(.vjs-control-bar) {
  background: transparent !important;
  background-color: transparent !important;
  background-image: none !important;
  margin-bottom: 20px;
}

/* Убираем фон для всех состояний плеера */
.player-wrap :deep(.video-js.vjs-has-started .vjs-control-bar),
.player-wrap :deep(.video-js.vjs-paused .vjs-control-bar),
.player-wrap :deep(.video-js.vjs-playing .vjs-control-bar) {
  background: transparent !important;
  background-color: transparent !important;
}


/* Базовый размер "плей-круга" в контрол-баре */
.player-wrap :deep(.vjs-play-control .vjs-icon-placeholder) {
  width: 20px;
  height: 20px;
}

/* Глушим встроенный глиф и рисуем свой SVG */
.player-wrap :deep(.vjs-play-control .vjs-icon-placeholder::before) {
  content: "";
  display: block;
  width: 100%;
  height: 100%;
  background-repeat: no-repeat;
  background-position: center;
  background-size: contain;
}

/* Когда видео на паузе — показываем иконку PLAY */
.player-wrap :deep(.vjs-paused .vjs-play-control .vjs-icon-placeholder::before),
.player-wrap :deep(.vjs-ended  .vjs-play-control .vjs-icon-placeholder::before) {
  background-image: url("/icons/play.svg");
  background-size: 16px;
}

/* Когда видео играет — показываем иконку PAUSE */
.player-wrap :deep(.vjs-playing .vjs-play-control .vjs-icon-placeholder::before) {
  background-image: url("/icons/pause.svg");
}


</style>
