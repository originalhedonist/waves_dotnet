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
                            <p>The number of rises in the track.</p>
                            <p>Set this to zero for the track to start at full amplitude.</p>
                        </v-tooltip>
                        <v-label>Number of rises</v-label>
                    </div>
                    <v-slider v-model="rises.count" min="0" max="20" thumb-label="always" />
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
                            <p>The percentage amplitude of each rise.</p>
                        </v-tooltip>
                        <v-label>Amount</v-label>
                    </div>
                    <v-slider v-model="rises.amount" min="0" max="1" step="0.01" thumb-label="always">
                        <template v-slot:thumb-label="p">
                            {{(p.value*100).toFixed(0)}}%
                        </template>
                    </v-slider>
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
                            <p>The time each rise takes, in seconds.</p>
                        </v-tooltip>
                        <v-label>Length each (seconds)</v-label>
                    </div>
                    <v-slider v-model="rises.lengthEachSeconds" min="5" max="120" thumb-label="always"></v-slider>
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
                            <p>The earliest in the track a rise can occur, in minutes.</p>
                        </v-tooltip>
                        <v-label>Earliest time (minutes)</v-label>
                    </div>
                    <v-slider v-model="rises.earliestTimeMinutes" min="0" max="60" thumb-label="always"></v-slider>
                </v-layout>
            </v-col>
        </v-row>
    </div>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model } from 'vue-property-decorator';
    import { Rises } from '../dtos';

    import '@/dtos';
    @Component
    export default class RisesEditor extends Vue {
        @Prop() public rises: Rises;
    }

</script>

<style scoped>
    
</style>
