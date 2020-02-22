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
        <GChart type="ScatterChart"
                :data="chartData"
                :options="chartOptions"/>
        <!--<div>Randomness: {{variance.randomness}}</div>-->
    </div>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model } from 'vue-property-decorator';
    import { client } from '../shared';
   
    import { Hello, CreateFileRequest, CreateFileRequestVariance } from '../dtos';
    import { GChart } from 'vue-google-charts';
    import * as _ from 'underscore';
    import { Debounce } from 'typescript-debounce';

    import '@/dtos';
    @Component({
        components: {
            GChart,
        },
    })
    export default class VarianceEditor extends Vue {
        @Prop() public variance: CreateFileRequestVariance;
        public chartData = [
                ['Time through track', 'Percentage from min to max value'],
                [1, 1],
                [2, 4],
                [3, 9],
                [4,16],
            ];

        @Debounce({millisecondsDelay: 500})
        @Watch('variance.progression')
        @Watch('variance.randomness')
        redrawChart() {
            var newChartData: [any] = [['Time through track', 'Percentage from min to max value']];
            const maxNumPoints = 1000;
            const p = this.variance.progression;
            const r = this.variance.randomness;
            for (var i = 1; i <= maxNumPoints; i++) {
                const progress = i / maxNumPoints;
                const randomnessComponent = Math.pow(Math.random(), r);
                const progressionComponent = Math.pow(progress, p);
                const desiredValue = randomnessComponent * progressionComponent;
                const newData = [progress, desiredValue];
                newChartData.push(newData);
            }
            this.chartData = newChartData;
        }

        get chartOptions() {
            return {
                chart: {
                    title: 'Company Performance',
                    subtitle: 'Sales, Expenses, and Profit: 2014-2017',
                }
            }
        }
    }

</script>

<style scoped>

</style>
