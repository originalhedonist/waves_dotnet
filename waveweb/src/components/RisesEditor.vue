<template>
    <div>
        <v-row>
            <v-col cols="12">
                <v-slider v-model="rises.count" min="0" max="20" label="Number of rises" thumb-label="always" />
            </v-col>
        </v-row>

        <v-row class="top-space">
            <v-col cols="12">
                <v-slider v-model="rises.amount" label="Amount" min="0" max="1" step="0.01" thumb-label="always">
                    <template v-slot:thumb-label="p">
                        {{(p.value*100).toFixed(0)}}%
                    </template>
                </v-slider>
            </v-col>
        </v-row>

        <v-row class="top-space">
            <v-col cols="12">
                <v-slider v-model="rises.lengthEachSeconds" label="Length each (seconds)" min="5" max="120" thumb-label="always"></v-slider>
            </v-col>
        </v-row>

        <v-row class="top-space">
            <v-col cols="12">
                <v-slider v-model="rises.earliestTimeMinutes" label="Earliest time (minutes)" min="0" max="60" thumb-label="always"></v-slider>
            </v-col>
        </v-row>
    </div>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model } from 'vue-property-decorator';
    import { Rises } from '../dtos';

    import '@/dtos';
    @Component
    export default class RisesEditor extends Vue {
        @Prop() public rises: Rises;
    }

</script>

<style scoped>
    
</style>
