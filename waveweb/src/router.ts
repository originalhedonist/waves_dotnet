import Vue from 'vue';
import Router from 'vue-router';

import Home from './home/Home.vue';
import View1 from './view1/View1.vue';
import View2 from './view2/View2.vue';

Vue.use(Router);

const routes = [
  { path: '/', component: Home, props: { name: 'Vue' } },
  { path: '*', redirect: '/' },
];

export default new Router ({
    mode: 'history',
    linkActiveClass: 'active',
    routes,
});
