<template>
    <div>
        <section>
            <v-slider :min="minSectionLength" max="300" v-model="channel.sections.sectionLengthSeconds" label="Section length (seconds)" thumb-label="always" />
        </section>

        <v-card class="controls-card">
            <h4>Ramp length</h4>

            <section>
                <v-range-slider v-model="channel.sections.rampLengthRange" label="Range (seconds)" thumb-label="always" :min="minRampLength" :max="maxRampLength"/>
            </section>

            <VarianceExpansionPanel :variance="channel.sections.rampLengthVariation" title="Variation" />

        </v-card>

        <v-card class="controls-card">
            <h4>Feature length</h4>

            <section>
                <v-range-slider v-model="channel.sections.featureLengthRange" :min="minFeatureLength" :max="maxFeatureLength" label="Range (seconds)" thumb-label="always"/>
            </section>

            <VarianceExpansionPanel :variance="channel.sections.featureLengthVariation" title="Feature length variation" />
        </v-card>

        <v-card class="controls-card">
            <GChart type="AreaChart" :data="chartData" :options="chartOptions"/>
        </v-card>

        <v-switch v-model="channel.useCustomWaveformExpression" label="Use custom waveform expression" />

        <div v-if="channel.useCustomWaveformExpression">
            <v-row>
                <v-text-field label="Waveform expression" :value="channel.waveformExpression" />
            </v-row>
            <v-row>
                <v-btn>Test</v-btn>
            </v-row>
        </div>
    </div>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model } from 'vue-property-decorator';
    import { client } from '../shared';

    import { CreateFileRequest, Variance, ChannelSettings } from '../dtos';
    import { GChart } from 'vue-google-charts';
    import * as _ from 'underscore';
    import { Debounce } from 'typescript-debounce';
    import VarianceExpansionPanel from '@/components/VarianceExpansionPanel.vue';

    import '@/dtos';
    @Component({
        components: {
            VarianceExpansionPanel,
        },
    })
    export default class ChannelEditor extends Vue {
        @Prop() public channel: ChannelSettings;
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
            return this.channel.sections.rampLengthRange[0] * 2 + this.channel.sections.featureLengthRange[0];
        }

        public get maxRampLength(): number {
            return Math.floor(this.channel.sections.sectionLengthSeconds / 2);
        }

        public get maxFeatureLength(): number {
            return this.channel.sections.sectionLengthSeconds;
        }

        @Debounce({ millisecondsDelay: 500 })
        @Watch('channel.sections.sectionLengthSeconds')
        @Watch('channel.sections.featureLengthRange')
        @Watch('channel.sections.rampLengthRange')
        public redrawVisualization() {
            this.redrawVisualizationNow();
        }

        public redrawVisualizationNow() {
            const newChartData: [any] = [['Time', 'Short', 'Long']];
            for (let seconds = 0; seconds <= this.channel.sections.sectionLengthSeconds; seconds += 0.5) {
                const short = this.getYVal(seconds,
                    this.channel.sections.rampLengthRange[0],
                    this.channel.sections.featureLengthRange[0], true);

                const long = this.getYVal(seconds,
                    this.channel.sections.rampLengthRange[1],
                    this.channel.sections.featureLengthRange[1], false);
                console.log('short = ', short, ' long = ', long);
                newChartData.push([seconds, short, long - short]);
            }
            this.chartData = newChartData;
        }

        private getYVal(x: number, rampLength: number, featureLength: number, doPrint: boolean) : number {
            if (x * 2 > this.channel.sections.sectionLengthSeconds) {
                return this.getYVal(this.channel.sections.sectionLengthSeconds - x, rampLength, featureLength, false); // mirror
            }
            
            const leadLength = (this.channel.sections.sectionLengthSeconds - featureLength) / 2 - rampLength;
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
                    { color: '#1976d2' }
                ]
            };
        }

    }



</script>

<style scoped>
    
</style>
