<template>
    <div>
        <v-slider v-model="variance.randomness"
                  label="Randomness"
                  thumb-label="always"
                  min="0"
                  max="1"
                  step="0.01" />
        <v-slider v-model="variance.progression"
                  label="Progression"
                  thumb-label="always"
                  min="0"
                  max="1"
                  step="0.01" />
    </div>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model } from 'vue-property-decorator';
    import { client } from '../shared';
   
    import { Hello, CreateFileRequest, CreateFileRequestVariance } from '../dtos';
    import { GChart } from 'vue-google-charts';

    import '@/dtos';
    @Component({
        components: {
            GChart
        }
    })
    export default class VarianceEditor extends Vue {
        @Prop() public variance: CreateFileRequestVariance;

    }

</script>

<style scoped>

</style>
