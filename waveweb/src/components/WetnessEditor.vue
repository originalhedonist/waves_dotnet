<template>
    <div>
        <v-row>
            <v-switch v-model="wetness.linkToFeature" label="Link to feature" />
        </v-row>
        <v-row>
            <v-range-slider v-model="wetness.amountRange" label="Range" min="0" max="1" step="0.01" thumb-label="always">
                <template v-slot:thumb-label="p">{{(p.value*100).toFixed(0)}}%</template>
            </v-range-slider>
        </v-row>
        <v-row>
            <VarianceExpansionPanel :variance="wetness.variation" title="Variation" />
        </v-row>
    </div>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model } from 'vue-property-decorator';
    import { Wetness } from '../dtos';
    import VarianceExpansionPanel from '@/components/VarianceExpansionPanel.vue';
    import '@/dtos';
    @Component({
        components: {
            VarianceExpansionPanel,
        },
    })
    export default class WetnessEditor extends Vue {
        @Prop() public wetness: Wetness;
    }

</script>

<style scoped>
    
</style>
