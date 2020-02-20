<template>
  <v-app>
      <router-view></router-view>
  </v-app>
</template>

<script lang="ts">
import Vue from 'vue';
import { Component } from 'vue-property-decorator';

@Component
export default class AppComponent extends Vue {
    public clipped = false;
    public drawer = false;
    public fixed = false;
    public items = [
      { icon: 'home', title: 'Home', to: '/' },
      { icon: 'bubble_chart', title: 'View 1', to: '/view1' },
      { icon: 'contacts', title: 'View 2', to: '/view2' },
    ];
    public miniVariant = false;
    public right = true;
    public title = 'waveweb';
    public name = '';
}
</script>