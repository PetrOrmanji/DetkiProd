import '@mdi/font/css/materialdesignicons.css'

import 'vuetify/styles'
import { createVuetify } from 'vuetify'
import { VVideo } from 'vuetify/labs/VVideo'

export default defineNuxtPlugin((app) => {
    const vuetify = createVuetify({
        components: {
            VVideo,
        },
    })
    app.vueApp.use(vuetify)
})