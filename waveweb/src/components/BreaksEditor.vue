<template>
    <div>
        <v-row align="center" class="top-space">
            <v-col cols="12">
                <v-layout horizontal>
                    <div>
                        <v-tooltip top>
                            <template v-slot:activator="{ on }">
                                <v-icon color="primary" v-on="on">mdi-information</v-icon>
                            </template>
                            The earliest time in the track a break can occur, in minutes
                        </v-tooltip>
                        <v-label>Earliest time (minutes)</v-label>
                    </div>
                    <v-slider v-model="breaks.minTimeSinceStartOfTrackMinutes" min="0" max="60" thumb-label="always" />
                </v-layout>
            </v-col>
        </v-row>


        <v-row align="center" class="top-space">
            <v-col cols="12">
                <v-layout horizontal>
                    <div>
                        <v-tooltip top>
                            <template v-slot:activator="{ on }">
                                <v-icon color="primary" v-on="on">mdi-information</v-icon>
                            </template>
                            The range of lengths a break can be, in seconds.
                        </v-tooltip>
                        <v-label>Length range (seconds)</v-label>
                    </div>
                    <v-range-slider v-model="breaks.lengthSecondsRange" min="1" max="120" thumb-label="always"></v-range-slider>
                </v-layout>
            </v-col>
        </v-row>

        <v-row align="center" class="top-space">
            <v-col cols="12">
                <v-layout horizontal>
                    <div>
                        <v-tooltip top>
                            <template v-slot:activator="{ on }">
                                <v-icon color="primary" v-on="on">mdi-information</v-icon>
                            </template>
                            The range of times between breaks, in minutes.
                        </v-tooltip>
                        <v-label>Time between (minutes)</v-label>
                    </div>
                    <v-range-slider v-model="breaks.timeBetweenBreaksMinutesRange" min="1" max="120" thumb-label="always"></v-range-slider>
                </v-layout>
            </v-col>
        </v-row>

        <v-row align="center" class="top-space">
            <v-col cols="12">
                <v-layout horizontal>
                    <div>
                        <v-tooltip top>
                            <template v-slot:activator="{ on }">
                                <v-icon color="primary" v-on="on">mdi-information</v-icon>
                            </template>
                            <p>The range of ramp lengths each break has.</p>
                            <p>This is the time it takes to transition from normal amplitude to zero, and also the time it takes to transition back to full amplitude again.</p>
                        </v-tooltip>
                        <v-label>Ramp length (seconds)</v-label>
                    </div>
                    <v-slider v-model="breaks.rampLengthSeconds" min="5" max="120" thumb-label="always"></v-slider>
                </v-layout>
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
