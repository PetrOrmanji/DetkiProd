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
      <img src="/labels/detkiBlack.svg" alt="logo" class="dialog-logo" />

      <v-btn variant="plain" class="buttons-url" :ripple="false" @click="onAllProjects">
        <img src="/labels/allProjects.svg" class="image" alt="all projects"/>
      </v-btn>
      <v-btn variant="plain" class="buttons-url" :ripple="false"
             href="https://www.instagram.com/detki.prod?igsh=MXcxbG90YjNvNXpqcQ==" >
        <img src="/labels/instagram.svg" class="image" alt="instagram"/>
      </v-btn>
      <v-btn variant="plain" class="buttons-url" :ripple="false"
             href="https://t.me/detkiprod">
        <img src="/labels/telegram.svg" class="image" alt="telegram"/>
      </v-btn>

    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
const props = defineProps<{ modelValue: boolean }>()
const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'open-projects'): void
}>()

function onUpdate(val: boolean) {
  emit('update:modelValue', val)
}

function onAllProjects() {
  // Close menu dialog and notify parent to open projects dialog
  emit('update:modelValue', false)
  emit('open-projects')
}
</script>

<style scoped>
.dialog-close {
  position: absolute;
  top:36px;
  right:36px;
}

.menu-card {
  background-color: rgba(238, 238, 238, 0.5);
}
.buttons-url{
  margin-bottom: 20vh;
}
.image{
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
