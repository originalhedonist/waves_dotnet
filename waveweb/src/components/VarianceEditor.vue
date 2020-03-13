<template>
    <div>
        <v-row align="center">
            <v-col cols="12">
                <v-layout horizontal>
                    <div>
                        <v-tooltip top>
                            <template v-slot:activator="{ on }">
                                <v-icon color="primary" v-on="on">mdi-information</v-icon>
                            </template>
                            <p><b>Progression</b> governs how the <b>maximum</b> value rises through the track:</p>
                            <p>To have just as high max values at the start of the track as the end, set progression to zero.</p>
                            <p>To have the max value rise linearly, set progression to 1.</p>
                            <p>To have the max value rising faster towards the end, set progression to greater than 1.</p>
                        </v-tooltip>
                        <v-label>Randomness</v-label>
                    </div>
                    <v-slider v-model="variance.progression"
                              thumb-label="always"
                              min="0"
                              max="2"
                              step="0.01" />
                </v-layout>
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
                            <p><b>Randomness</b> governs how likely the expected values are to be near the maximum.</p>
                            <p>To have the values always at the maximum, set randomness to zero.</p>
                            <p>To have the values evenly distributed between the minimum value and the maximum, set randomness to 1.</p>
                        </v-tooltip>
                        <v-label>Randomness</v-label>
                    </div>
                    <v-slider v-model="variance.randomness"
                              thumb-label="always"
                              min="0"
                              max="1"
                              step="0.01" />
                </v-layout>
            </v-col>
        </v-row>

        <GChart type="ScatterChart"
                :data="chartData"
                :options="chartOptions" />
    </div>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model } from 'vue-property-decorator';
    import { client } from '../shared';

    import { CreateFileRequest, Variance } from '../dtos';
    import { GChart } from 'vue-google-charts';
    import { Debounce } from 'typescript-debounce';

    import '@/dtos';
    @Component({
        components: {
            GChart,
        },
    })
    export default class VarianceEditor extends Vue {
        @Prop() public variance: Variance;
        public chartData = [
            ['x', 'y'],
            [0, 0],
        ];
        public chartOptions = {
            legend: 'none',
            pointSize: 5,
        };

        public mounted() {
            this.redrawChartNow();
        }

        @Debounce({ millisecondsDelay: 500 })
        @Watch('variance.progression')
        @Watch('variance.randomness')
        public redrawChart() {
            this.redrawChartNow();
        }

        public redrawChartNow() {
            const newChartData: [any] = [['x', 'y']];
            const maxNumPoints = 1000;
            const p = this.variance.progression;
            const r = this.variance.randomness;
            for (let i = 1; i <= maxNumPoints; i++) {
                const progress = i / maxNumPoints;
                const randomnessComponent = Math.pow(Math.random(), r);
                const progressionComponent = Math.pow(progress, p);
                const desiredValue = randomnessComponent * progressionComponent;
                const newData = [progress * 100, desiredValue * 100];
                newChartData.push(newData);
            }
            this.chartData = newChartData;
        }
    }

</script>

<style scoped>
</style>
