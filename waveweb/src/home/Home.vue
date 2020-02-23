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
                            <v-switch v-model="Request.dualChannel" label="Dual channel"></v-switch>
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
                    <ChannelEditor :channel="Request.channel0" />
                </v-expansion-panel-content>
            </v-expansion-panel>

            <v-expansion-panel v-show="Request.dualChannel" transition="v-guff-y-transition">
                <v-expansion-panel-header>Right channel settings</v-expansion-panel-header>
                <v-expansion-panel-content>
                    <ChannelEditor :channel="Request.channel0" />
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
    import { CreateFileRequest, ChannelSettings, Variance, Sections, FeatureProbability } from '../dtos';
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
                    featureLengthRange: [10, 20],
                    rampLengthRange: [2, 5],
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
            }),
            channel1: new ChannelSettings({
                sections: new Sections({
                    sectionLengthSeconds: 30,
                    featureLengthRange: [10, 20],
                    rampLengthRange: [2, 5],
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