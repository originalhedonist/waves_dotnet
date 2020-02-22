<template>
    <div>
        <v-switch v-model="channel.useCustomWaveformExpression" label="Use custom waveform expression"/>

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

    import '@/dtos';
    @Component({
        components: {
        },
    })
    export default class ChannelEditor extends Vue {
        @Prop() public channel: ChannelSettings;
    }

</script>

<style scoped>
</style>
