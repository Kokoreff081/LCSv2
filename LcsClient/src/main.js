import './assets/main.css'
import * as $ from "jquery";
import { createI18n } from 'vue-i18n'
window.$ = window.jQuery = $;//require('jquery');
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import { dom, library } from '@fortawesome/fontawesome-svg-core';
import { fas } from '@fortawesome/free-solid-svg-icons'
import { fab } from '@fortawesome/free-brands-svg-icons';
import { far } from '@fortawesome/free-regular-svg-icons';
import { faLock, faEnvelope } from '@fortawesome/free-solid-svg-icons';
import router from './router'; // <---
import store from './stores/store';
import VueCookies from 'vue-cookies';
import App from './App.vue';
library.add(fas, fab, far, faLock, faEnvelope);
dom.watch()
import VueKonva from 'vue-konva';
import PrimeVue from 'primevue/config';
import DialogService from 'primevue/dialogservice';
//import adminlte scripts
import "../node_modules/admin-lte/dist/js/adminlte.js"
import "../node_modules/admin-lte/plugins/select2/js/select2.full.min.js"
import "../node_modules/admin-lte/plugins/bootstrap/js/bootstrap.bundle.min.js"
import "../node_modules/admin-lte/plugins/overlayScrollbars/js/jquery.overlayScrollbars.min.js"
import "../node_modules/admin-lte/plugins/daterangepicker/daterangepicker.js"
import "../node_modules/admin-lte/plugins/jquery-knob/jquery.knob.min.js"
import "../node_modules/admin-lte/plugins/sparklines/sparkline.js"
import "../node_modules/admin-lte/plugins/chart.js/Chart.min.js"
import "../node_modules/admin-lte/plugins/jquery/jquery.min.js"
import "../node_modules/admin-lte/plugins/jquery-ui/jquery-ui.min.js"
import "../node_modules/admin-lte/plugins/inputmask/jquery.inputmask.min.js"
import "../node_modules/admin-lte/plugins/bootstrap-switch/js/bootstrap-switch.min.js"
import "../node_modules/admin-lte/plugins/bs-stepper/js/bs-stepper.min.js"
import "../node_modules/admin-lte/plugins/dropzone/min/dropzone.min.js"
//import adminlte styles
import '../node_modules/admin-lte/dist/css/adminlte.min.css'
import "../node_modules/admin-lte/plugins/overlayScrollbars/css/OverlayScrollbars.min.css"
import "../node_modules/admin-lte/plugins/summernote/summernote-bs4.min.css"
import "../node_modules/admin-lte/plugins/daterangepicker/daterangepicker.css"
import "../node_modules/admin-lte/plugins/jqvmap/jqvmap.min.css"
import "../node_modules/admin-lte/plugins/icheck-bootstrap/icheck-bootstrap.min.css"
import "../node_modules/admin-lte/plugins/tempusdominus-bootstrap-4/css/tempusdominus-bootstrap-4.min.css"
import "../node_modules/admin-lte/plugins/bootstrap4-duallistbox/bootstrap-duallistbox.min.css"
import "../node_modules/admin-lte/plugins/bs-stepper/css/bs-stepper.min.css"
import "../node_modules/admin-lte/plugins/dropzone/min/dropzone.min.css"

import { createApp } from 'vue'


const app = createApp(App)

app.use(router).use(store)
app.use(lang).use(PrimeVue).use(DialogService).use(VueKonva).use(VueCookies)
app.mount('#app')
