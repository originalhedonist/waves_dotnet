<template>
    <div>
        <v-row>
            <v-col cols="1">Frequency</v-col>
            <v-col cols="11">
                <v-slider v-model="probability.frequencyWeighting" min="0" max="1" step="0.01" :thumb-label="true" />
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="1">Wetness</v-col>
            <v-col cols="11">
                <v-slider v-model="probability.wetnessWeighting" min="0" max="1" step="0.01" :thumb-label="true" />
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="1">Nothing</v-col>
            <v-col cols="11">
                <v-slider v-model="probability.nothingWeighting" min="0" max="1" step="0.01" :thumb-label="true" />
            </v-col>
        </v-row>
    </div>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model } from 'vue-property-decorator';
    import { FeatureProbability } from '../dtos';

    import '@/dtos';
    @Component
    export default class FeatureProbabilityEditor extends Vue {
        @Prop() public probability: FeatureProbability;
    }



</script>

<style scoped>
    table { width: 100%; }
</style>
