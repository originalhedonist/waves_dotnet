<template>
    <div>
        <v-row>
            <v-slider min="1" max="300" v-model="channel.sections.sectionLengthSeconds" label="Section length (seconds)" thumb-label="always" />
        </v-row>

        <v-card class="controls-card">
            <h4>Ramp length</h4>

            <section>
                <v-range-slider v-model="channel.sections.rampLengthRange" label="Range (seconds)" thumb-label="always" min="1" max="5" />
            </section>

            <VarianceExpansionPanel :variance="channel.sections.rampLengthVariation" title="Variation" />

        </v-card>

        <v-card class="controls-card">
            <h4>Feature length</h4>

            <section>
                <v-range-slider v-model="channel.sections.featureLengthRange" min="0" max="300" label="Range (seconds)" thumb-label="always"/>
            </section>

            <VarianceExpansionPanel :variance="channel.sections.featureLengthVariation" title="Feature length variation" />
        </v-card>


        <v-switch v-model="channel.useCustomWaveformExpression" label="Use custom waveform expression" />

        <div v-if="channel.useCustomWaveformExpression">
            <v-row>
                <v-text-field label="Waveform expression" :value="channel.waveformExpression" />
            </v-row>
            <v-row>
                <v-btn>Test</v-btn>
            </v-row>
        </div>
    </div>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model } from 'vue-property-decorator';
    import { client } from '../shared';

    import { CreateFileRequest, Variance, ChannelSettings } from '../dtos';
    import { GChart } from 'vue-google-charts';
    import * as _ from 'underscore';
    import { Debounce } from 'typescript-debounce';
    import VarianceExpansionPanel from '@/components/VarianceExpansionPanel.vue';

    import '@/dtos';
    @Component({
        components: {
            VarianceExpansionPanel,
        },
    })
    export default class ChannelEditor extends Vue {
        @Prop() public channel: ChannelSettings;
        public minRampLength: number = 1;
        public minFeatureLength: number = 0;

        //public get maxRampLength(): number {
        //    return Math.floor(this.channel.sections.sectionLengthSeconds / 2);
        //}

        //public get maxFeatureLength(): number {
        //    return this.channel.sections.sectionLengthSeconds;
        //}
    }



</script>

<style scoped>
    
</style>
