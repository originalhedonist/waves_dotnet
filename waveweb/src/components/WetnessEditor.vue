<template>
    <div>
        
    </div>
</template>

<script lang="ts">

    import Vue from 'vue';
    import { Component, Prop, Watch, Model } from 'vue-property-decorator';
    import { Wetness } from '../dtos';

    import '@/dtos';
    @Component
    export default class WetnessEditor extends Vue {
        @Prop() public wetness: Wetness;
    }

</script>

<style scoped>
    
</style>
