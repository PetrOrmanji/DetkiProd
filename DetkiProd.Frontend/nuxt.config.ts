// https://nuxt.com/docs/api/configuration/nuxt-config
import { defineNuxtConfig } from 'nuxt/config'
import vuetify, { transformAssetUrls } from 'vite-plugin-vuetify'
export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  runtimeConfig: {
    public: {
      mainVideoUrl: process.env.NUXT_PUBLIC_MAIN_VIDEO_URL || 'http://localhost:5000/api/files/downloadmain',
      projectsUrl: process.env.NUXT_PUBLIC_PROJECTS_URL || 'http://localhost:5000/api/project/getprojects',
    },
  },
  build: {
    transpile: ['vuetify'],
  },
  modules: [
    (_options, nuxt) => {
      nuxt.hooks.hook('vite:extendConfig', (config) => {
        if (!config.plugins) config.plugins = []
        config.plugins.push(vuetify({ autoImport: true }))
      })
    },
    //...
  ],
  vite: {
    vue: {
      template: {
        transformAssetUrls,
      },
    },
  },
})
