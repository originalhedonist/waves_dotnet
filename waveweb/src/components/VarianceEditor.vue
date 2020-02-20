<template>
    <v-container>
        <div>Editing variance</div>
        <div>Randomness: {{variance.randomness}}</div>
        <div>Progression: {{variance.progression}}</div>
    </v-container>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model } from 'vue-property-decorator';
    import { client } from '../shared';
   
    import { Hello, CreateFileRequest, CreateFileRequestVariance } from '../dtos';
    import '@/dtos';
    @Component({})
    export default class VarianceEditor extends Vue {
        @Prop() public variance: CreateFileRequestVariance;

    }

</script>

<style scoped>

</style>
