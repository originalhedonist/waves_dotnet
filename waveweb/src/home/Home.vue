<template>
    
    <v-container fluid>
        <v-expansion-panels :multiple="true" >
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

        </v-expansion-panels>
        <div style="padding:12px; margin-top: 50px">
            <v-row>
                <v-slider v-model="chunks" label="Ten second chunks" min="1" max="200" step="1" thumb-label="always"/>
            </v-row>
            <v-row>
                <v-btn @click="test" :disabled="creatingFile">Test</v-btn>
            </v-row>
            <v-row>
                <span>{{serverMessage}}</span>
            </v-row>
            <v-row v-show="jobProgressModel.jobId !== null">
                <JobProgress :model="jobProgressModel" @complete="jobComplete"/>
            </v-row>
        </div>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Prop, Watch } from 'vue-property-decorator';
    import { client } from '../shared';
    import { CreateFileRequest, TestRequest, Variance } from '../dtos';
    import ChannelEditor from '../components/ChannelEditor.vue';
    import DefaultDataCreator from '../defaultdatacreator';
    import JobProgress from '../components/JobProgress.vue';
    import JobProgressModel from '../jobprogressodel';

    @Component({
        components: {
            ChannelEditor,
            JobProgress,
        },
    })
    export default class HomeComponent extends Vue {
        @Prop() public name: string;
        public jobProgressModel: JobProgressModel = new JobProgressModel({
            jobId: null,
        });
        public txtName: string = this.name;
        public result: string = '';
        public show: boolean = false;
        public creatingFile: boolean = false;
        public serverMessage: string = '';
        public chunks: number = 60;

        public Request: CreateFileRequest = new CreateFileRequest({
            trackLengthMinutes: 20,
            channel0: DefaultDataCreator.createDefaultChannelSettings(),
            channel1: DefaultDataCreator.createDefaultChannelSettings(),
        });

        public async test() {
            this.creatingFile = true;
            const request = new TestRequest({
                chunks: this.chunks,
            });
            const response = await client.post(request);
            this.serverMessage = response.message;
            this.jobProgressModel.jobId = response.jobId;
        }

        public jobComplete() {
            this.serverMessage = 'The job completed';
            this.jobProgressModel.jobId = null;
            this.creatingFile = false;
        }
    }
</script>

<style lang="scss">
    @import '../app.scss';

    .result {
        margin: 10px;
        color: darken($navbar-background, 10%);
    }
</style>