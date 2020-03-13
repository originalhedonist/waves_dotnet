<template>
    <div>
        <v-row>
            <v-col cols="12">
                This can be a function of:
                <ul>
                    <li><b>t</b> = time through track</li>
                    <li><b>T</b> = total track time</li>
                    <li><b>V</b> = feature value (1 in the middle of the rise of a section, 0 at the beginning/end of a section (see the graph in the 'section length' section).</li>
                </ul>
            </v-col>
        </v-row>
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
