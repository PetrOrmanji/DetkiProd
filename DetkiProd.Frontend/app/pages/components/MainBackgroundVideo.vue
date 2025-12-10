
<template>
  <video
      class="bg-video"
      preload="metadata"
      playsinline
      autoplay
      muted
      loop
  >
    <source :src="src" type="video/webm">
  </video>
</template>
<script setup lang="ts">
import {useRuntimeConfig} from "#imports";
const config = useRuntimeConfig()
const src = config.public.mainVideoUrl || '/video/bg.webm'
</script>

<style scoped>
.bg-video {
  position: fixed;
  inset: 0;               /* top:0; right:0; bottom:0; left:0 */
  width: 100vw;
  height: 100vh;
  object-fit: cover;      /* растянуть на весь экран без искажений */
  pointer-events: none;   /* клики проходят сквозь видео */
}

/* уважение системной настройки «меньше анимаций» */
@media (prefers-reduced-motion: reduce) {
  .bg-video { display: none; }
}
</style>