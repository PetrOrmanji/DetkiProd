<template>
  <div class="page-root">
    <div v-if="!menu" class="click-catcher" @click="menu = true"></div>
    <MainBackgroundVideo/>
    <img v-if="!menu && !projects && !video" src="/labels/detkiWhite.svg" alt="detki" class="centered-image" />
    <MenuIcon v-if="!menu && !projects && !video"/>
    <MenuDialog v-model="menu" @open-projects="onOpenProjects"/>
    <ProjectsDialog v-model="projects" @open-video="onOpenVideo" @back="backFromProject"/>
    <VideoPlayer v-model="video" :project-id="selectedId" :project="selectedProject"/>
  </div>
</template>

<script setup lang="ts">
import MainBackgroundVideo from "~/pages/components/MainBackgroundVideo.vue";
import MenuIcon from "~/pages/components/icons/MenuIcon.vue";
import MenuDialog from "~/pages/components/MenuDialog.vue";
import {ref} from 'vue'
import ProjectsDialog from "~/pages/components/ProjectsDialog.vue";
import VideoPlayer from "~/pages/components/VideoPlayer.vue";

const menu = ref(false);
const projects = ref(false);
const video = ref(false);
const selectedId = ref<string | number | undefined>(undefined)
const selectedProject = ref<any | null>(null)

function onOpenProjects() {
  projects.value = true
  menu.value = false
}

function backFromProject() {
  projects.value = false
  menu.value = true
}

function onOpenVideo(project: any) {
  selectedId.value = project?.id
  selectedProject.value = project || null
  video.value = true
  // Keep projects open so closing the video returns to AllProjects automatically
}

</script>

<style scoped>

.click-catcher {
  position: fixed;
  inset: 0; /* top:0; right:0; bottom:0; left:0 */
  z-index: 10;
}

.centered-image {
  position: fixed;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);
  max-width: 90vw;
  max-height: 90vh;
  pointer-events: auto;
}


.page-root :deep(*) {
  /* не обязательно, только если какой-то элемент перекрывает ловушку */
}
</style>