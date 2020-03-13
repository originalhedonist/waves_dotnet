<template>
    <v-expansion-panels>
        <v-expansion-panel>
            <v-expansion-panel-header>
                <v-row align="center">

                    <span>Section length</span>

                    <v-tooltip top>
                        <template v-slot:activator="{ on }">
                            <v-icon style="margin-left:10px" color="primary" v-on="on">mdi-information</v-icon>
                        </template>
                        <div>The track is composed of multiple 'sections', of the same length.</div>
                        <div>Each may 'rise' in the middle.</div>
                        <div>The rise affects either wetness or pulse frequency, but not both.</div>
                        <div>Which it affects is governed by the 'Feature Choice' settings (below)</div>
                    </v-tooltip>
                </v-row>
            </v-expansion-panel-header>
            <v-expansion-panel-content>
                <SectionEditor :sections="channel.sections" />
            </v-expansion-panel-content>
        </v-expansion-panel>

        <v-expansion-panel>
            <v-expansion-panel-header>
                <v-row align="center">

                    <span>Pulse frequency</span>

                    <v-tooltip top>
                        <template v-slot:activator="{ on }">
                            <v-icon style="margin-left:10px" color="primary" v-on="on">mdi-information</v-icon>
                        </template>
                        <div>The frequency of the pulses within the track.</div>
                        <div>The 'High frequency range' switch simply changes the limits of the sliders - use this for a fairly 'percussive' track.</div>
                    </v-tooltip>
                </v-row>

            </v-expansion-panel-header>
            <v-expansion-panel-content>
                <PulseFrequencyEditor :frequency="channel.pulseFrequency" />
            </v-expansion-panel-content>
        </v-expansion-panel>

        <v-expansion-panel>
            <v-expansion-panel-header>
                <v-row align="center">

                    <span>Waveform</span>

                    <v-tooltip top>
                        <template v-slot:activator="{ on }">
                            <v-icon style="margin-left:10px" color="primary" v-on="on">mdi-information</v-icon>
                        </template>
                        <div>Whether to use a custom waveform expression for the pulses.</div>
                        <div>If not, it will simply use sin(x) for left, and either cos(x) or sin(x) for right, depending on the 'Phase Shift Pulses' setting in 'Track Details'.</div>
                    </v-tooltip>
                </v-row>
            </v-expansion-panel-header>
            <v-expansion-panel-content>
                <v-switch v-model="channel.useCustomWaveformExpression" label="Use custom waveform expression" />
                <div v-if="channel.useCustomWaveformExpression">
                    <v-row>
                        <v-col cols="12">
                            The expression operates on <b>x</b> and can use any of the <a target="_blank" href="http://mathparser.org/mxparser-math-collection/unary-functions/">functions</a> and <a target="_blank" href="http://mathparser.org/mxparser-math-collection/operators/">operators</a> in this library.
                        </v-col>
                    </v-row>
                    <v-row>
                        <v-col cols="12">
                            <v-text-field label="Waveform expression" v-model="channel.waveformExpression"
                                          :error-messages="waveformExpressionError" />
                        </v-col>
                    </v-row>
                    <v-row>
                        <v-col cols="12">
                            <v-btn @click="testWaveformExpression" style="margin-right:20px">Test</v-btn>
                            <v-progress-circular v-if="testingWaveformExpression" :indeterminate="true" />
                        </v-col>
                    </v-row>
                    <template v-if="showWaveformDemoCharts">
                        <v-row v-if="testingWaveformExpression">
                            <v-col cols="12">
                                <v-skeleton-loader />
                            </v-col>
                        </v-row>

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
            <v-expansion-panel-header>
                <v-row align="center">

                    <span>Carrier frequency</span>

                    <v-tooltip top>
                        <template v-slot:activator="{ on }">
                            <v-icon style="margin-left:10px" color="primary" v-on="on">mdi-information</v-icon>
                        </template>
                        <span>The carrier frequency for the channel (either static value or expression)</span>
                    </v-tooltip>
                </v-row>
            </v-expansion-panel-header>
            <v-expansion-panel-content>
                <CarrierFrequencyEditor :carrierFrequency="channel.carrierFrequency" :dualChannel="dualChannel" :isRight="isRight" />
            </v-expansion-panel-content>
        </v-expansion-panel>

        <v-expansion-panel>
            <v-expansion-panel-header>
                <v-row align="center">

                    <span>Feature choice</span>

                    <v-tooltip top>
                        <template v-slot:activator="{ on }">
                            <v-icon style="margin-left:10px" color="primary" v-on="on">mdi-information</v-icon>
                        </template>
                        <div>The relative likelihood of the wetness or frequency rising.</div>
                        <div>A section won't ever make both wetness and frequency rise at once but will choose one or the other according to these settings.</div>
                        <div>The probability of each is its weighting divided by the total of all weightings.</div>
                        <div>(To always have either wetness or frequency rise, set the 'nothing' weighting to zero.)</div>
                        <div></div>
                    </v-tooltip>
                </v-row>
            </v-expansion-panel-header>
            <v-expansion-panel-content>
                <FeatureProbabilityEditor :probability="channel.featureProbability" />
            </v-expansion-panel-content>
        </v-expansion-panel>

        <v-expansion-panel>
            <v-expansion-panel-header>
                <v-row align="center">

                    <span>Wetness</span>

                    <v-tooltip top>
                        <template v-slot:activator="{ on }">
                            <v-icon style="margin-left:10px" color="primary" v-on="on">mdi-information</v-icon>
                        </template>
                        <div>The wetness of the track, and its dependence on the progression through the track and randomness</div>
                        <div>0% wetness is a perfect sine wave, 100% is a constant tone.</div>
                    </v-tooltip>
                </v-row>
            </v-expansion-panel-header>
            <v-expansion-panel-content>
                <WetnessEditor :wetness="channel.wetness" />
            </v-expansion-panel-content>
        </v-expansion-panel>

        <v-expansion-panel>
            <v-expansion-panel-header>
                <v-row align="center">

                    <span>Breaks</span>

                    <v-tooltip top>
                        <template v-slot:activator="{ on }">
                            <v-icon style="margin-left:10px" color="primary" v-on="on">mdi-information</v-icon>
                        </template>
                        <div>Breaks within the track. A break is a period of silence before the track resumes.</div>
                    </v-tooltip>
                </v-row>
            </v-expansion-panel-header>

            <v-expansion-panel-content>
                <BreaksEditor :breaks="channel.breaks" />
            </v-expansion-panel-content>
        </v-expansion-panel>

        <v-expansion-panel>
            <v-expansion-panel-header>
                <v-row align="center">

                    <span>Rises</span>

                    <v-tooltip top>
                        <template v-slot:activator="{ on }">
                            <v-icon style="margin-left:10px" color="primary" v-on="on">mdi-information</v-icon>
                        </template>
                        <p>Rises within the track. A rise is where the track's maximum amplitude increases over a certain period of time, and stays there.</p>
                        <p>(If you have any rises, then this inevitably means the track starts at less than full amplitude in order to do this.)</p>
                    </v-tooltip>
                </v-row>
            </v-expansion-panel-header>
            <v-expansion-panel-content>
                <RisesEditor :rises="channel.rises" />
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
    import CarrierFrequencyEditor from '@/components/CarrierFrequencyEditor.vue';
    import WetnessEditor from '@/components/WetnessEditor.vue';
    import BreaksEditor from '@/components/BreaksEditor.vue';
    import RisesEditor from '@/components/RisesEditor.vue';

    import '@/dtos';
    @Component({
        components: {
            SectionEditor,
            FeatureProbabilityEditor,
            PulseFrequencyEditor,
            CarrierFrequencyEditor,
            WetnessEditor,
            BreaksEditor,
            RisesEditor,
        },
    })
    export default class ChannelEditor extends Vue {
        @Prop() public channel: ChannelSettings;
        @Prop() public dualChannel: boolean;
        @Prop() public isRight: boolean;

        public testingWaveformExpression: boolean = false;
        public waveformExpressionError: string|null = null;
        public waveformDemoChartOptionsNoFeature = {
            legend: 'none',
            curveType: 'function',
            title: 'No feature',
            vAxes: [{viewWindowMode: 'maximized'}],
        };
        public waveformDemoChartOptionsHighFrequency = {
            legend: 'none',
            curveType: 'function',
            title: 'High frequency feature',
            vAxes: [{viewWindowMode: 'maximized'}],
        };
        public waveformDemoChartOptionsLowFrequency = {
            legend: 'none',
            curveType: 'function',
            title: 'Low frequency feature',
            vAxes: [{viewWindowMode: 'maximized'}],
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
            this.showWaveformDemoCharts = false;
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
                // tslint:disable-next-line:no-console
                console.error(error);
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
