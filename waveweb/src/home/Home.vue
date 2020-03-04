<template>
    
    <v-container fluid>
        <v-expansion-panels :multiple="true">
            <v-expansion-panel>
                <v-expansion-panel-header>
                    Track details
                </v-expansion-panel-header>
                <v-expansion-panel-content>
                    <v-row>
                        <v-switch v-model="Request.randomization" label="Use randomization"/>
                    </v-row>
                    <v-row>
                        <v-slider v-model="Request.trackLengthMinutes"
                                  label="Track length (minutes)"
                                  thumb-label="always"
                                  min="1"
                                  max="60" />

                    </v-row>

                    <v-row>
                        <v-col cols="4">
                            <v-switch v-model="Request.dualChannel" label="Independent channels"></v-switch>
                       </v-col>
                        <v-col cols="4">
                            <v-tooltip top>
                                <template v-slot:activator="{ on }">
                                    <v-switch v-on="on" :disabled="!Request.dualChannel" v-model="Request.phaseShiftCarrier" label="Phase shift carrier"></v-switch>
                                </template>
                                <div>Setting this flag means left uses sin(x) and right cos(x).</div>
                            </v-tooltip>

                        </v-col>

                        <v-col cols="4">
                            <v-tooltip top>
                                <template v-slot:activator="{ on }">
                                    <v-switch v-on="on" :disabled="!Request.dualChannel" v-model="Request.phaseShiftPulses" label="Phase shift pulses"></v-switch>
                                </template>
                                <div>Setting this flag means left uses sin(x) and right cos(x).</div>
                                <div>(Ignored if custom waveform expression is provided however.)</div>

                            </v-tooltip>

                        </v-col>
                    </v-row>

                </v-expansion-panel-content>
            </v-expansion-panel>

            <v-expansion-panel>
                <v-expansion-panel-header>{{Request.dualChannel ? 'Left channel settings' : 'Channel settings'}}</v-expansion-panel-header>
                <v-expansion-panel-content>
                    <ChannelEditor :channel="Request.channel0" :isRight="false" :dualChannel="Request.dualChannel" />
                </v-expansion-panel-content>
            </v-expansion-panel>

            <v-expansion-panel v-show="Request.dualChannel" transition="v-guff-y-transition">
                <v-expansion-panel-header>Right channel settings</v-expansion-panel-header>
                <v-expansion-panel-content>
                    <ChannelEditor :channel="Request.channel1" :isRight="true" :dualChannel="Request.dualChannel" />
                </v-expansion-panel-content>
            </v-expansion-panel>

            <!--<v-expansion-panel>
        <v-expansion-panel-header>
            Section details
        </v-expansion-panel-header>
        <v-expansion-panel-content>
            <VarianceEditor :variance="Request.channel0.featureLengthVariation"/>
        </v-expansion-panel-content>
    </v-expansion-panel>-->
        </v-expansion-panels>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Prop, Watch } from 'vue-property-decorator';
    import { client } from '../shared';
    import { CreateFileRequest, ChannelSettings, Variance, Sections, FeatureProbability, PulseFrequency, CarrierFrequency, Breaks, Rises, Wetness } from '../dtos';
    import '@/dtos';
    import VarianceEditor from '../components/VarianceEditor.vue';
    import ChannelEditor from '../components/ChannelEditor.vue';
    
    @Component({
        components: {
            ChannelEditor,
        },
    })
    export default class HomeComponent extends Vue {
        @Prop() public name: string;
        public txtName: string = this.name;
        public result: string = '';
        public show: boolean = false;

        public Request: CreateFileRequest = new CreateFileRequest({
            trackLengthMinutes: 20,
            channel0: new ChannelSettings({
                sections: new Sections({
                    sectionLengthSeconds: 30,
                    featureLengthRangeSeconds: [10, 20],
                    rampLengthRangeSeconds: [2, 5],
                    rampLengthVariation: new Variance({
                        progression: 0.7,
                        randomness: 0.3,
                    }),
                    featureLengthVariation: new Variance({
                        progression: 0.7,
                        randomness: 0.3,
                    }),
                }),
                featureProbability: new FeatureProbability({
                    frequencyWeighting: 1.0,
                    wetnessWeighting: 0.2,
                    nothingWeighting: 0.8,
                }),
                pulseFrequency: new PulseFrequency({
                    quiescent: 0.8,
                    low: 0.4,
                    high: 2.0,
                    chanceOfHigh: 0.6,
                    variation: new Variance({
                        progression: 0.7,
                        randomness: 0.3,
                    }),
                }),
                carrierFrequency: new CarrierFrequency({
                    left: '800',
                    right: '800',
                }),
                breaks: new Breaks({
                    lengthSecondsRange: [10, 60],
                    minTimeSinceStartOfTrackMinutes: 10,
                    timeBetweenBreaksMinutesRange: [2, 20],
                    rampLengthSeconds: 20,
                }),
                rises: new Rises({
                    count: 2,
                    earliestTimeMinutes: 10,
                    amount: 0.08,
                    lengthEachSeconds: 20,
                }),
                wetness: new Wetness({
                    amountRange: [0.4, 0.6],
                    linkToFeature: true,
                    variation: new Variance({
                        progression: 0.7,
                        randomness: 0.3,
                    }),
                }),
            }),
            channel1: new ChannelSettings({
                sections: new Sections({
                    sectionLengthSeconds: 30,
                    featureLengthRangeSeconds: [10, 20],
                    rampLengthRangeSeconds: [2, 5],
                    rampLengthVariation: new Variance({
                        progression: 0.7,
                        randomness: 0.3,
                    }),
                    featureLengthVariation: new Variance({
                        progression: 0.7,
                        randomness: 0.3,
                    }),
                }),
                featureProbability: new FeatureProbability({
                    frequencyWeighting: 1.0,
                    wetnessWeighting: 0.2,
                    nothingWeighting: 0.8,
                }),
                pulseFrequency: new PulseFrequency({
                    quiescent: 0.8,
                    low: 0.4,
                    high: 2.0,
                    chanceOfHigh: 0.6,
                    variation: new Variance({
                        progression: 0.7,
                        randomness: 0.3,
                    }),
                }),
                carrierFrequency: new CarrierFrequency({
                    left: '800',
                    right: '800',
                }),
                breaks: new Breaks({
                    lengthSecondsRange: [10, 60],
                    minTimeSinceStartOfTrackMinutes: 10,
                    timeBetweenBreaksMinutesRange: [2, 20],
                    rampLengthSeconds: 20,
                }),
                rises: new Rises({
                    count: 2,
                    earliestTimeMinutes: 10,
                    amount: 0.08,
                    lengthEachSeconds: 20,
                }),
                wetness: new Wetness({
                    amountRange: [0.4, 0.6],
                    linkToFeature: true,
                    variation: new Variance({
                        progression: 0.7,
                        randomness: 0.3,
                    }),
                }),
            }),
        });
    }
</script>

<style lang="scss">
    @import '../app.scss';

    .result {
        margin: 10px;
        color: darken($navbar-background, 10%);
    }
</style>