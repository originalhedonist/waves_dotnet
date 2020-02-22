<template>
    <div>
        Channel editor.
    </div>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model } from 'vue-property-decorator';
    import { client } from '../shared';

    import { CreateFileRequest, CreateFileRequestVariance, CreateFileRequestChannelSettings } from '../dtos';
    import { GChart } from 'vue-google-charts';
    import * as _ from 'underscore';
    import { Debounce } from 'typescript-debounce';

    import '@/dtos';
    @Component({
        components: {
        },
    })
    export default class ChannelEditor extends Vue {
        @Prop() public channel: CreateFileRequestChannelSettings;
    }

</script>

<style scoped>
</style>
