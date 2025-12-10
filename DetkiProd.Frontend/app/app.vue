<template>
  <NuxtLayout>
    <v-app>
      <NuxtPage />
    </v-app>
  </NuxtLayout>
</template>
<style>
html {
  scrollbar-width: none; /* Firefox */
}

html::-webkit-scrollbar {
  display: none; /* Chrome, Safari, Edge */
}
body {
  cursor: url("/icons/cursor.svg") 0 0, auto;
}
</style>
