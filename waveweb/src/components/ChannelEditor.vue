<template>
    <v-expansion-panels>
        <v-expansion-panel>
            <v-expansion-panel-header>Sections</v-expansion-panel-header>
            <v-expansion-panel-content>
                <SectionEditor :sections="channel.sections" />
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

    import '@/dtos';
    @Component({
        components: {
            SectionEditor,
            FeatureProbabilityEditor,
        },
    })
    export default class ChannelEditor extends Vue {
        @Prop() public channel: ChannelSettings;

        public testingWaveformExpression: boolean = false;
        public waveformExpressionError: string|null = null;

        @Watch('channel.waveformExpression')
        public onWaveformExpressionChanged() {
            this.waveformExpressionError = null;
        }

        public async testWaveformExpression() {
            this.testingWaveformExpression = true;
            const testWaveformRequest = new TestPulseWaveformRequest({
                sectionLengthSeconds: this.channel.sections.sectionLengthSeconds,
                waveformExpression: this.channel.waveformExpression,
            });
            const result = await client.post(testWaveformRequest);
            try {
                if (result.errorMessage) {
                    this.waveformExpressionError = result.errorMessage;
                } else if (result.success) {
                    this.waveformExpressionError = null;
                    // do the data...
                }
            }
            finally {
                this.testingWaveformExpression = false;
            }
        }
    }
</script>

<style scoped>
    
</style>
