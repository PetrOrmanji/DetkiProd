<template>
  <v-dialog
      :model-value="modelValue"
      @update:model-value="onUpdate"
      fullscreen
  >
    <v-card variant="text" class="menu-card d-flex align-center justify-center">
      <v-btn class="dialog-close" :ripple="false" variant="plain" @click="emit('update:modelValue', false)">
        <img src="/icons/close.svg"/>
      </v-btn>

      <v-btn class="back-btn"  variant="plain" :ripple="false" @click="emit('back')">
        <img src="/icons/back.svg"/>
      </v-btn>

      <img src="/labels/detkiBlack.svg" alt="logo" class="dialog-logo"/>
      <img src="/labels/allProjects.svg" alt="logo" class="center-image"/>
  <v-virtual-scroll
      v-if="projects"
      :items="projects"
      max-height="60vh"
  >
        <template v-slot:default="{ item }">
          <v-card class="pa-4 item-list" variant="plain" :ripple="false" @click="onClickItem(item)">
            <v-row no-gutters class="align-start">
              <!-- ЛЕВАЯ ЧАСТЬ -->
              <v-col cols="12" md="10" class="pr-md-4">
                <div class="title-box">
                  {{ item.name || item.title }}
                </div>
              </v-col>

              <!-- ПРАВАЯ ЧАСТЬ -->
              <v-col cols="12" md="2" class="d-flex flex-column justify-space-between align-end right-col">
                <div class="date">[{{ (item.date || item.year || '').toString().slice(0,4) }}]</div>
                <div class="utils">{{ Array.isArray(item.tools) ? item.tools.join(', ') : item.tools }}</div>
              </v-col>
            </v-row>
          </v-card>
        </template>
      </v-virtual-scroll>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import {projects as staticProjects} from '~/data/projects'
import {onMounted} from 'vue'
import {useRuntimeConfig} from '#imports'
const props = defineProps<{ modelValue: boolean }>()
const emit = defineEmits<{ (e: 'update:modelValue', value: boolean): void; (e: 'open-video', project: any): void ; (e:'back') : void}>()
const projects = ref<any[] | null>(null)
const loading = ref(true)

async function loadProjects() {
  const {public: {projectsUrl}} = useRuntimeConfig()
  try {
    const data: any = await $fetch(projectsUrl)
    if (Array.isArray(data) && data.length) {
      projects.value = data
      return
    }
  } catch (e) {
    // fallback below
  }
  projects.value = staticProjects
}

function onUpdate(val: boolean) {
  emit('update:modelValue', val)
}

function onClickItem(item: any) {
  if (!item) return
  emit('open-video', item)
}

onMounted(async () => {
  await loadProjects()
  loading.value = false
})
</script>

<style scoped>
.v-virtual-scroll::-webkit-scrollbar {
  display: none; /* Chrome, Safari */
}

.v-virtual-scroll {
  -ms-overflow-style: none;  /* IE и Edge */
  scrollbar-width: none;     /* Firefox */
}

.item-list {
  width: 40vw;
  cursor: pointer;
}

@media (max-width: 1920px) {
  .item-list {
    width: 53.3vw;
  }
}

@media (max-width: 1600px) {
  .item-list {
    width: 60vw;
  }
}

@media (max-width: 1300px) {
  .item-list {
    width: 70vw;
  }
}

.title-box {
  display: inline-block;
  border: 1.5px solid rgba(0, 0, 0, .7);
  font-family: Georgia, serif; /* как в примере */
  font-size: clamp(24px, 4vw, 50px);
  letter-spacing: 0.2em;
  line-height: 0.7;
  color: rgba(0, 0, 0, .85);
  transform: scaleY(1.4);
}

.menu-card {
  background-color: rgba(238, 238, 238, 0.5);
  position: relative;
}

.dialog-close {
  position: absolute;
  top:36px;
  right:36px;
}

.back-btn{
  position: absolute;
  top: 36px;
  left: 36px;
  z-index: 2;
}
.date {
  margin-bottom: 20px;
}

.center-image {
  display: block;
  margin-left: auto;
  margin-right: auto;
  margin-bottom: 20px;
}

.buttons-url {
  margin-bottom: 20vh;
}

.image {
  height: 20vh;
}

.dialog-logo {
  position: absolute;
  top: 20px;
  left: 50%;
  transform: translateX(-50%);
  height: 4vh;
  z-index: 2;
  pointer-events: none;
}
</style>
