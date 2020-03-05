<template>

    <v-container fluid>
        <v-expansion-panels :multiple="true">
            <v-expansion-panel>
                <v-expansion-panel-header>
                    Track details
                </v-expansion-panel-header>
                <v-expansion-panel-content>
                    <v-row>
                        <v-switch v-model="Request.randomization" label="Use randomization" />
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

        <div style="padding:12px">
            <v-row>
                <v-btn @click="createFile" :disabled="creatingFile">Create file</v-btn>
            </v-row>
            <v-row>
                <span v-show="creatingFile">File is being created. Depending on how long you have chosen it be, this might take a while. It should download automatically when complete.</span>
            </v-row>
            <v-row v-show="jobProgressModel.jobId !== null">
                <JobProgress :model="jobProgressModel" @complete="jobComplete" style="padding: 12px" />
            </v-row>
            <v-row v-show="fileDownloadLink !== null">
                <a :href="fileDownloadLink">The file was created. Click here to try to download it if it did not download automatically.</a>
            </v-row>
        </div>

        <div style="padding:12px">
            <v-row>
                <v-slider v-model="chunks" label="Ten second chunks" min="1" max="200" step="1" thumb-label="always" />
            </v-row>
            <v-row>
                <v-btn @click="test" :disabled="runningTest">Test</v-btn>
            </v-row>
            <v-row>
                <span>{{testServerMessage}}</span>
            </v-row>
            <v-row v-show="testJobProgressModel.jobId !== null">
                <JobProgress :model="testJobProgressModel" @complete="testJobComplete" style="padding: 12px" />
            </v-row>
            <div style="margin-top: 50px" v-show="testFileDownloadLink !== null">
                <a :href="testFileDownloadLink">Click here to try to download the file if it does not download automatically</a>
            </div>
            <div style="margin-top: 50px">
                <a href="/testdownload">Click here to (try to) download a file!</a>
            </div>
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
        public testJobProgressModel: JobProgressModel = new JobProgressModel({
            jobId: null,
        });
        public jobProgressModel: JobProgressModel = new JobProgressModel({
            jobId: null,
        });
        public txtName: string = this.name;
        public result: string = '';
        public show: boolean = false;
        public runningTest: boolean = false;
        public creatingFile: boolean = false;
        public testServerMessage: string = '';
        public serverMessage: string = '';
        public chunks: number = 60;
        public testFileDownloadLink: string | null = null;
        public fileDownloadLink: string | null = null;

        public Request: CreateFileRequest = new CreateFileRequest({
            trackLengthMinutes: 20,
            channel0: DefaultDataCreator.createDefaultChannelSettings(),
            channel1: DefaultDataCreator.createDefaultChannelSettings(),
        });

        public async test() {
            this.runningTest = true;
            const request = new TestRequest({
                chunks: this.chunks,
            });
            const response = await client.post(request);
            this.testServerMessage = response.message;
            this.testJobProgressModel.jobId = response.jobId;
        }

        public async createFile() {
            this.creatingFile = true;
            const response = await client.post(this.Request);
            this.jobProgressModel.jobId = response.jobId; // start polling
        }

        public testJobComplete() {
            this.testServerMessage = 'File created successfully!';
            this.testFileDownloadLink = '/downloadfile/' + this.testJobProgressModel.jobId;
            window.location.href = this.testFileDownloadLink;
            this.testJobProgressModel.jobId = null;
            this.runningTest = false;
        }

        public jobComplete() {
            this.serverMessage = 'File created successfully!';
            this.fileDownloadLink = '/downloadfile/' + this.jobProgressModel.jobId;
            window.location.href = this.fileDownloadLink; // triggers the download (doesn't navigate away)
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