<template>
    <div>Editing variance.</div>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch } from 'vue-property-decorator';
    import { client } from '../shared';
   
    import { Hello, CreateFileRequest } from '../dtos';
    import '@/dtos';
    @Component({})
    export default class VarianceEditor extends Vue {
    }
  //  export default {
  //name: 'VarianceEditor',
  //props: {
  //  msg: String,
  //},
//};

</script>

<style scoped>

</style>
