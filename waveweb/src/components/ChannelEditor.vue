<template>
    <v-expansion-panels>
        <v-expansion-panel>
            <v-expansion-panel-header>Sections</v-expansion-panel-header>
            <v-expansion-panel-content>
                <SectionEditor :sections="channel.sections" />
            </v-expansion-panel-content>
        </v-expansion-panel>

        <v-expansion-panel>
            <v-expansion-panel-header>Pulse frequency</v-expansion-panel-header>
            <v-expansion-panel-content>
                <PulseFrequencyEditor :frequency="channel.pulseFrequency"/>
            </v-expansion-panel-content>
        </v-expansion-panel>

        <v-expansion-panel>
            <v-expansion-panel-header>Waveform</v-expansion-panel-header>
            <v-expansion-panel-content>
                <v-switch v-model="channel.useCustomWaveformExpression" label="Use custom waveform expression"/>
                <div v-if="channel.useCustomWaveformExpression">
                    <v-row>
                        <v-text-field label="Waveform expression" v-model="channel.waveformExpression"
                                      :error-messages="waveformExpressionError"/>
                    </v-row>
                    <v-row>
                        <v-btn @click="testWaveformExpression" style="margin-right:20px">Test</v-btn>
                        <v-progress-circular v-if="testingWaveformExpression" :indeterminate="true"/>
                    </v-row>
                    <template v-if="showWaveformDemoCharts">
                        <v-row>
                            <v-col cols="12">
                                <GChart type="LineChart" :data="waveformDemoDataNoFeature" :options="waveformDemoChartOptionsNoFeature" />
                            </v-col>
                        </v-row>
                        <v-row>
                            <v-col cols="12">
                                <GChart type="LineChart" :data="waveformDemoDataHighFrequency" :options="waveformDemoChartOptionsHighFrequency" />
                            </v-col>
                        </v-row>
                        <v-row>
                            <v-col cols="12">
                                <GChart type="LineChart" :data="waveformDemoDataLowFrequency" :options="waveformDemoChartOptionsLowFrequency" />
                            </v-col>
                        </v-row>
                    </template>
                </div>
            </v-expansion-panel-content>
        </v-expansion-panel>

        <v-expansion-panel>
            <v-expansion-panel-header>Feature choice</v-expansion-panel-header>
            <v-expansion-panel-content>
                <FeatureProbabilityEditor :probability="channel.featureProbability"/>
            </v-expansion-panel-content>
        </v-expansion-panel>
    </v-expansion-panels>


</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model } from 'vue-property-decorator';
    import { client } from '../shared';

    import { CreateFileRequest, Variance, ChannelSettings, TestPulseWaveformRequest } from '../dtos';
    import { GChart } from 'vue-google-charts';
    import { Debounce } from 'typescript-debounce';
    import SectionEditor from '@/components/SectionEditor.vue';
    import FeatureProbabilityEditor from '@/components/FeatureProbabilityEditor.vue';
    import PulseFrequencyEditor from '@/components/PulseFrequencyEditor.vue';

    import '@/dtos';
    @Component({
        components: {
            SectionEditor,
            FeatureProbabilityEditor,
            PulseFrequencyEditor,
        },
    })
    export default class ChannelEditor extends Vue {
        @Prop() public channel: ChannelSettings;

        public testingWaveformExpression: boolean = false;
        public waveformExpressionError: string|null = null;
        public waveformDemoChartOptionsNoFeature = {
            legend: 'none',
            curveType: 'function',
            title: 'No feature',
        };
        public waveformDemoChartOptionsHighFrequency = {
            legend: 'none',
            curveType: 'function',
            title: 'High frequency feature',
        };
        public waveformDemoChartOptionsLowFrequency = {
            legend: 'none',
            curveType: 'function',
            title: 'Low frequency feature',
        };

        public showWaveformDemoCharts: boolean = false;
        public waveformDemoDataNoFeature: any[][] = [['t', 'y']];
        public waveformDemoDataHighFrequency: any[][] = [['t', 'y']];
        public waveformDemoDataLowFrequency: any[][] = [['t', 'y']];

        @Watch('channel.waveformExpression')
        public onWaveformExpressionChanged() {
            this.waveformExpressionError = null;
        }

        public async testWaveformExpression() {
            this.testingWaveformExpression = true;
            const testWaveformRequest = new TestPulseWaveformRequest({
                sections: this.channel.sections,
                pulseFrequency: this.channel.pulseFrequency,
                waveformExpression: this.channel.waveformExpression,
            });
            try {
                const result = await client.post(testWaveformRequest);
                if (result.errorMessage) {
                    this.waveformExpressionError = result.errorMessage;
                } else if (result.success) {
                    this.waveformExpressionError = null;
                    this.showWaveformDemoCharts = true;
                    this.waveformDemoDataNoFeature = this.makeNewDemoData(result.sampleNoFeature);
                    this.waveformDemoDataHighFrequency = this.makeNewDemoData(result.sampleHighFrequency);
                    this.waveformDemoDataLowFrequency = this.makeNewDemoData(result.sampleLowFrequency);
                }
            }
            catch (error) {
                this.waveformExpressionError = 'An unknown error occurred on the server.';
            }
            finally {
                this.testingWaveformExpression = false;
            }
        }

        private makeNewDemoData(newData: number[][]) {
            let retVal: any[][] = [['t', 'n']];
            retVal = retVal.concat(newData);
            return retVal;
        }
    }
</script>

<style scoped>
    
</style>
