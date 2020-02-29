<template>
    <div>
        <section>
            <v-slider :min="minSectionLength" max="300" v-model="sections.sectionLengthSeconds" label="Section length (seconds)" thumb-label="always" />
        </section>

        <v-card class="controls-card">
            <h4>Ramp length</h4>

            <section>
                <v-range-slider v-model="sections.rampLengthRangeSeconds" label="Range (seconds)" thumb-label="always" :min="minRampLength" :max="maxRampLength"/>
            </section>

            <VarianceExpansionPanel :variance="sections.rampLengthVariation" title="Ramp length variation" />

        </v-card>

        <v-card class="controls-card">
            <h4>Feature length</h4>

            <section>
                <v-range-slider v-model="sections.featureLengthRangeSeconds" :min="minFeatureLength" :max="maxFeatureLength" label="Range (seconds)" thumb-label="always"/>
            </section>

            <VarianceExpansionPanel :variance="sections.featureLengthVariation" title="Feature length variation" />
        </v-card>

        <v-card class="controls-card">
            <GChart type="AreaChart" :data="chartData" :options="chartOptions"/>
        </v-card>
    </div>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model } from 'vue-property-decorator';
    import { client } from '../shared';

    import { Sections } from '../dtos';
    import { GChart } from 'vue-google-charts';
    import { Debounce } from 'typescript-debounce';
    import VarianceExpansionPanel from '@/components/VarianceExpansionPanel.vue';

    import '@/dtos';
    @Component({
        components: {
            VarianceExpansionPanel,
        },
    })
    export default class ChannelEditor extends Vue {
        @Prop() public sections: Sections;
        public minRampLength: number = 1;
        public chartData = [
            ['Time', 'Short', 'Long'],
            [0, 0, 0],
        ];
        public minFeatureLength: number = 0;

        public mounted() {
            this.redrawVisualizationNow();
        }

        public get minSectionLength(): number {
            return this.sections.rampLengthRangeSeconds[0] * 2 + this.sections.featureLengthRangeSeconds[0];
        }

        public get maxRampLength(): number {
            return Math.floor(this.sections.sectionLengthSeconds / 2);
        }

        public get maxFeatureLength(): number {
            return this.sections.sectionLengthSeconds - 2 * this.minRampLength;
        }

        @Debounce({ millisecondsDelay: 500 })
        @Watch('sections.sectionLengthSeconds')
        @Watch('sections.featureLengthRangeSeconds')
        @Watch('sections.rampLengthRangeSeconds')
        public redrawVisualization() {
            this.redrawVisualizationNow();
        }

        public redrawVisualizationNow() {
            const newChartData: [any] = [['Time', 'Short', 'Long']];
            const minFeatureLength = Math.min(
                this.sections.featureLengthRangeSeconds[0],
                this.sections.sectionLengthSeconds - this.sections.rampLengthRangeSeconds[0] * 2); // min usable feature length
            const maxFeatureLength = Math.min(
                this.sections.featureLengthRangeSeconds[1],
                this.sections.sectionLengthSeconds - this.sections.rampLengthRangeSeconds[0] * 2); // max usable feature length

            const minRampLength = this.sections.rampLengthRangeSeconds[0]; // this is the one thing that can't be reduced
            const maxRampLength = Math.min(
                Math.min(this.sections.rampLengthRangeSeconds[1], (this.sections.sectionLengthSeconds - minFeatureLength) / 2),
                this.sections.rampLengthRangeSeconds[0]); // therefore max ramp length can't be less than minRampLength either
            for (let seconds = 0; seconds <= this.sections.sectionLengthSeconds; seconds += 0.5) {
                const short = this.getYVal(seconds, minRampLength, minFeatureLength);
                const long = this.getYVal(seconds, maxRampLength, maxFeatureLength);
                newChartData.push([seconds, short, long - short]);
            }
            this.chartData = newChartData;
        }

        private getYVal(x: number, rampLength: number, featureLength: number): number {
            if (x * 2 > this.sections.sectionLengthSeconds) {
                return this.getYVal(this.sections.sectionLengthSeconds - x, rampLength, featureLength); // mirror
            }

            const leadLength = Math.max((this.sections.sectionLengthSeconds - featureLength) / 2 - rampLength, 0);
            const rawVal = (x - leadLength) / rampLength;
            if (rawVal < 0) {
                return 0;
            }
            else if (rawVal > 1) {
                return 1;
            }
            else {
                return rawVal;
            }
        }

        get chartOptions() {
            return {
                isStacked: true,
                legend: 'none',
                series: [
                    { color: 'white', lineWidth: 0 },
                    { color: '#1976d2' },
                ],
            };
        }

    }



</script>

<style scoped>
    
</style>
