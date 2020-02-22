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
        <GChart type="ColumnChart"
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

    import '@/dtos';
    @Component({
        components: {
            GChart,
        },
    })
    export default class VarianceEditor extends Vue {
        @Prop() public variance: CreateFileRequestVariance;

        mounted() {
            console.log('mounted, progression = ', this.variance.progression, ' randomness = ', this.variance.randomness);
        }

        get chartData() {
            console.info('returning new chart data');
            return [
                ['Year', 'Sales', 'Expenses', 'Profit'],
                ['2014', 1000, this.variance.randomness * 1000, 200],
                ['2015', 1170, this.variance.randomness * 1170, 250],
                ['2016', 660, this.variance.randomness * 660, 300],
                ['2017', 1030, this.variance.randomness * 1030, 350]
            ];
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
