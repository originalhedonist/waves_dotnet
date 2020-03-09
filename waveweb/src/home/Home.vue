<template>

    <v-container fluid>
        <v-expansion-panels :multiple="true">
            <v-expansion-panel>
                <v-expansion-panel-header>
                    Track details
                </v-expansion-panel-header>
                <v-expansion-panel-content>
                    <v-row>
                        <v-col cols="12">
                            <v-switch v-model="Request.randomization" label="Use randomization" />
                        </v-col>
                    </v-row>
                    <v-row>
                        <v-col cols="12">
                            <v-slider v-model="Request.trackLengthMinutes"
                                      label="Track length (minutes)"
                                      thumb-label="always"
                                      min="1"
                                      max="60" />
                        </v-col>
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

            <v-expansion-panel v-show="Request.dualChannel">
                <v-expansion-panel-header>Right channel settings</v-expansion-panel-header>
                <v-expansion-panel-content>
                    <ChannelEditor :channel="Request.channel1" :isRight="true" :dualChannel="Request.dualChannel" />
                </v-expansion-panel-content>
            </v-expansion-panel>

        </v-expansion-panels>

        <v-card style="margin-top: 20px">
            <v-card-text>
                <div>
                    <v-btn @click="createFile" :disabled="creatingFile">Create file</v-btn>
                    <span style="color: red; margin-left:20px;" v-show="errorMessage !== null">{{errorMessage}}</span>
                </div>
                <div v-show="creatingFile" class="top-space">
                    <div style="float:left">
                        <img src="@/assets/img/tea.svg" alt="Go and make a cup of tea" style="width: 100px; height: 100px" />
                    </div>
                    <div style="float:left">
                        <p>File is being created.</p>
                        <p>Depending on how long you have chosen it be, this might take a while.</p>
                        <p>It should download automatically when complete.</p>
                    </div>
                    <div style="clear:both"/>
                </div>
                <div v-show="jobProgressModel.jobId !== null">
                    <JobProgress :model="jobProgressModel" @complete="jobComplete" style="padding: 12px" />
                </div>
                <div v-show="fileDownloadLink !== null" class="top-space">
                    <a :href="fileDownloadLink">The file was created. Click here to try to download it if it did not download automatically.</a>
                </div>
            </v-card-text>
        </v-card>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import { Component, Prop, Watch } from 'vue-property-decorator';
    import { client } from '../shared';
    import { CreateFileRequest, TestRequest, Variance, JobProgressStatus } from '../dtos';
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
        public testJobProgressModel: JobProgressModel = new JobProgressModel({
            jobId: null,
        });
        public jobProgressModel: JobProgressModel = new JobProgressModel({
            jobId: null,
        });
        public txtName: string = this.name;
        public result: string = '';
        public show: boolean = false;
        public creatingFile: boolean = false;
        public fileDownloadLink: string | null = null;
        public errorMessage: string | null = null;

        public Request: CreateFileRequest = new CreateFileRequest({
            trackLengthMinutes: 20,
            channel0: DefaultDataCreator.createDefaultChannelSettings(),
            channel1: DefaultDataCreator.createDefaultChannelSettings(),
        });

        public async createFile() {
            this.creatingFile = true;
            const response = await client.post(this.Request);
            this.jobProgressModel.jobId = response.jobId; // start polling
        }

        public jobComplete(status: JobProgressStatus, message: string) {
            if (status === JobProgressStatus.Failed) {
                this.errorMessage = message;
            }
            if (status === JobProgressStatus.Complete) {
                this.fileDownloadLink = '/downloadfile/' + this.jobProgressModel.jobId;
                window.location.href = this.fileDownloadLink; // triggers the download (doesn't navigate away)
            }
            this.jobProgressModel.jobId = null; // stops it polling
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