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
                  max="2"
                  step="0.01" />
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
