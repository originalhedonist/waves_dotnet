<template>
    <div>
        <v-row>
            <v-col cols="12">
                <v-switch v-model="isHigh" label="High frequency range" />
            </v-col>
        </v-row>
        <v-row align="center">
            <v-col cols="12">
                <v-layout horizontal>
                    <div>
                        <v-tooltip top>
                            <template v-slot:activator="{ on }">
                                <v-icon color="primary" v-on="on">mdi-information</v-icon>
                            </template>
                            <div>Frequency of the pulses outside of the rise of a section.</div>
                        </v-tooltip>
                        <v-label>Quiescent</v-label>
                    </div>
                    <v-slider v-model="frequency.quiescent" :min="isHigh ? 5: 0.01" :max="isHigh ? 50 : 5" :step="isHigh ? 0.1 : 0.01" thumb-label="always" />
                </v-layout>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="12">
                <v-layout horizontal>
                    <div>
                        <v-tooltip top>
                            <template v-slot:activator="{ on }">
                                <v-icon color="primary" v-on="on">mdi-information</v-icon>
                            </template>
                            <div>Frequency of the pulses at the top of the rise of a 'high frequency' section.</div>
                        </v-tooltip>
                        <v-label>High</v-label>
                    </div>
                    <v-slider v-model="frequency.high" :min="isHigh ? 5: 0.01" :max="isHigh ? 50 : 5" :step="isHigh ? 0.1 : 0.01" thumb-label="always" />
                </v-layout>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="12">
                <v-layout horizontal>
                    <div>
                        <v-tooltip top>
                            <template v-slot:activator="{ on }">
                                <v-icon color="primary" v-on="on">mdi-information</v-icon>
                            </template>
                            <div>Frequency of the pulses at the top of the rise of a 'low frequency' section.</div>
                        </v-tooltip>
                        <v-label>Low</v-label>
                    </div>
                    <v-slider v-model="frequency.low" :min="isHigh ? 5: 0.01" :max="isHigh ? 50 : 5" :step="isHigh ? 0.1 : 0.01" thumb-label="always" />
                </v-layout>
            </v-col>
        </v-row>

        <v-row>
            <v-col cols="12">
                <v-layout horizontal>
                    <div>
                        <v-tooltip top>
                            <template v-slot:activator="{ on }">
                                <v-icon color="primary" v-on="on">mdi-information</v-icon>
                            </template>
                            <div>How likely a section is to be a 'high frequency' one.</div>
                        </v-tooltip>
                        <v-label>Chance of high</v-label>
                    </div>
                    <v-slider v-model="frequency.chanceOfHigh" :min="0" :max="1" :step="0.01" thumb-label="always" />
                </v-layout>
            </v-col>
        </v-row>

    </div>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model } from 'vue-property-decorator';
    import { PulseFrequency } from '../dtos';

    import '@/dtos';
    @Component
    export default class PulseFrequencyEditor extends Vue {
        @Prop() public frequency: PulseFrequency;
        public isHigh: boolean = false;

        @Watch('frequency.quiescent')
        public quiescentChanged() {
            if (this.frequency.quiescent > this.frequency.high) {
                this.frequency.high = this.frequency.quiescent;
            }
            if (this.frequency.quiescent < this.frequency.low) {
                this.frequency.low = this.frequency.quiescent;
            }
        }

        @Watch('frequency.low')
        public lowChanged() {
            if (this.frequency.low > this.frequency.quiescent) {
                this.frequency.quiescent = this.frequency.low;
                // don't need to modify high as quiescent watcher will in turn modify it
            }
        }

        @Watch('frequency.high')
        public highChanged() {
            if (this.frequency.high < this.frequency.quiescent) {
                this.frequency.quiescent = this.frequency.high;
                // don't need to modify low as quiescent watcher will in turn modify it
            }
        }

    }



</script>

<style scoped>
    table {
        width: 100%;
    }
</style>
