<template>
    
    <v-container fluid>
        <v-expansion-panels :multiple="true">
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

                    <transition name="fade">
                        <div v-if="Request.dualChannel">
                            <v-row>
                                <v-switch v-model="Request.phaseShiftCarrier" label="Phase shift carrier frequency"></v-switch>
                            </v-row>
                            <v-row>
                                <v-switch v-model="Request.phaseShiftPulses" label="Phase shift pulses"></v-switch>
                            </v-row>
                        </div>
                    </transition>


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
    import { CreateFileRequest, CreateFileRequestChannelSettings, CreateFileRequestVariance } from '../dtos';
    import '@/dtos';
    import VarianceEditor from '../components/VarianceEditor.vue';
    import ChannelEditor from '../components/ChannelEditor.vue';
    
    @Component({
        components: {
            VarianceEditor,
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