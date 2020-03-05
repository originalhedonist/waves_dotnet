<template>
    <v-row>
        <v-progress-linear :value="progressPercentage" />
    </v-row>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model, Emit } from 'vue-property-decorator';
    import { client } from '../shared';

    import '@/dtos';
    import { JobProgressRequest } from '@/dtos';
    import JobProgressModel from '../jobprogressodel';

    @Component
    export default class JobProgress extends Vue {
        @Prop() public model: JobProgressModel;

        public progressPercentage: number = 0;
        private intervalId: NodeJS.Timeout| null = null;

        @Watch('model.jobId')
        public async onJobIdChanged() {
            if (this.model.jobId !== null) {
                this.intervalId = setInterval(this.poll, 10000);
            }
            await this.poll();
        }

        private async poll() {
            if (this.model.jobId !== null) {
                const pollRequest = new JobProgressRequest({
                    jobId: this.model.jobId,
                });
                const pollResponse = await client.get(pollRequest);
                if (pollResponse.progress > 0) {
                    this.progressPercentage = pollResponse.progress * 100;
                }
                if (pollResponse.isComplete) {
                    if (this.intervalId !== null) {
                        clearInterval(this.intervalId);
                    }
                    this.$emit('complete');
                }
            }
        }

    }



</script>

<style scoped>
</style>
