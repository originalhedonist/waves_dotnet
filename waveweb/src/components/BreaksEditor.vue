<template>
    <div>
        <v-row>
            <v-col cols="12">
                <v-slider v-model="breaks.minTimeSinceStartOfTrackMinutes" label="Earliest time (minutes)" min="0" max="60" thumb-label="always" />
            </v-col>
        </v-row>

        <v-row class="top-space">
            <v-col cols="12">
                <v-range-slider v-model="breaks.timeBetweenBreaksMinutesRange" label="Time between each (minutes)" min="0" max="60" thumb-label="always" />
            </v-col>
        </v-row>

        <v-row class="top-space">
            <v-col cols="12">
                <v-range-slider v-model="breaks.lengthSecondsRange" label="Length of each (seconds)" min="1" max="120" thumb-label="always"></v-range-slider>
            </v-col>
        </v-row>

        <v-row class="top-space">
            <v-col cols="12">
                <v-slider v-model="breaks.rampLengthSeconds" label="Ramp length (seconds)" min="5" max="120" thumb-label="always"></v-slider>
            </v-col>
        </v-row>
    </div>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model } from 'vue-property-decorator';
    import { Breaks } from '../dtos';

    import '@/dtos';
    @Component
    export default class BreaksEditor extends Vue {
        @Prop() public breaks: Breaks;
    }



</script>

<style scoped>
    
</style>
