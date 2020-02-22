<template>
    
    <v-container fluid>
        <v-expansion-panels >
            <v-expansion-panel>
                <v-expansion-panel-header>
                    Track details
                </v-expansion-panel-header>
                <v-expansion-panel-content>
                    <v-row>
                        <v-slider v-model="Request.trackLengthMinutes"
                                  label="Track length (minutes)"
                                  thumb-label="always"
                                  min="1"
                                  max="60" />

                    </v-row>

                    <v-row>
                        <v-switch v-model="Request.dualChannel" label="Dual channel"></v-switch>
                    </v-row>
                    <v-slide-y-transition>
                        <div v-show="Request.dualChannel">
                            <v-row>
                                <v-switch v-model="Request.phaseShiftCarrier" label="Phase shift carrier frequency"></v-switch>
                            </v-row>
                            <v-row>
                                <v-switch v-model="Request.phaseShiftPulses" label="Phase shift pulses"></v-switch>
                            </v-row>
                        </div> 
                    </v-slide-y-transition>


                </v-expansion-panel-content>
            </v-expansion-panel>
            <v-expansion-panel>
                <v-expansion-panel-header>
                    Section details
                </v-expansion-panel-header>
                <v-expansion-panel-content>
                    <VarianceEditor :variance="Request.channel0.featureLengthVariation"/>
                </v-expansion-panel-content>
            </v-expansion-panel>
        </v-expansion-panels>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Prop, Watch } from 'vue-property-decorator';
    import { client } from '../shared';
    import { CreateFileRequest, CreateFileRequestChannelSettings, CreateFileRequestVariance } from '../dtos';
    import '@/dtos';
    import VarianceEditor from '../components/VarianceEditor.vue';
    
    @Component({
        components: {
            VarianceEditor,
        },
    })
    export default class HomeComponent extends Vue {
        @Prop() public name: string;
        public txtName: string = this.name;
        public result: string = '';
        public Request: CreateFileRequest = new CreateFileRequest({
            trackLengthMinutes: 20,
            channel0: new CreateFileRequestChannelSettings({
                featureLengthVariation: new CreateFileRequestVariance({
                    progression: 0.7,
                    randomness: 0.3,
                }),
            }),
            channel1: new CreateFileRequestChannelSettings({
                featureLengthVariation: new CreateFileRequestVariance({
                    progression: 0.6,
                    randomness: 0.4,
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