import '@babel/polyfill';
import 'font-awesome/css/font-awesome.css';
import './app.scss';
import 'es6-shim';

import Vue from 'vue';
import vuetify from './plugins/vuetify';
import App from './App.vue';
import VueGoogleCharts from 'vue-google-charts';

import router from './router';
import '@mdi/font/css/materialdesignicons.css';

Vue.use(VueGoogleCharts);

const app = new Vue({
    el: '#app',
    render: (h) => h(App),
    router,
    vuetify,
});
