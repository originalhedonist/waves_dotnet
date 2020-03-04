<template>
    <div>
        <v-row>
            <template v-if="!dualChannel">
                <v-col cols="4">
                    <v-text-field label="Left channel carrier frequency expression" v-model="carrierFrequency.left" />
                </v-col>
                <v-col cols="4">
                    <v-text-field label="Right channel carrier frequency expression" v-model="carrierFrequency.right" />
                </v-col>
            </template>

            <template v-if="dualChannel">
                <v-col cols="4" v-if="!isRight">
                    <v-text-field label="Carrier frequency expression" v-model="carrierFrequency.left" />
                </v-col>
                <v-col cols="4" v-if="isRight">
                    <v-text-field label="Carrier frequency expression" v-model="carrierFrequency.right" />
                </v-col>
            </template>
        </v-row>
    </div>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model } from 'vue-property-decorator';
    import { CarrierFrequency } from '../dtos';
    import '@/dtos';
    @Component
    export default class CarrierFrequencyEditor extends Vue {
        @Prop() public carrierFrequency: CarrierFrequency;
        @Prop() public dualChannel: boolean;
        @Prop() public isRight: boolean;

    }

</script>

<style scoped>
    
</style>
