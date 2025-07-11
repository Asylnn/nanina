<script lang="ts">

import StatModifier from '@/classes/modifiers/stat_modifier';
import Waifu from '@/classes/waifu/waifu';

export default {
    name : "WaifuStatDisplayComponent",
    data() {
        return {
            StatModifier: StatModifier,
        }
    },
    props: {
        statName: {
            type : String,
            required : true,
        },
        statAmount: {
            type : Number,
            required : true,
        },
        waifu: {
            type : Waifu,
            required : true
        },
        statModifier: {
            type : StatModifier,
            required : false
        },
    }
}

</script>

<template>
    <div class="stat">
        <span>{{ $t(statName) }}</span> 
        <span v-if="statModifier == StatModifier.CritChance || statModifier == StatModifier.CritDamage">{{ Math.floor(statAmount*1000)/10 }}%</span> 
        <span v-else>{{ Math.floor(statAmount) }}</span> 
        <span class="modifier" v-if="statModifier == StatModifier.CritChance || statModifier == StatModifier.CritDamage" >(+{{Math.floor(waifu.GetAdditiveModificators(statModifier)*1000)/10}})</span>
        <span class="modifier" v-else >({{waifu.DisplayAdditiveModificator(statModifier)}})</span> 

        <span class="modifier">({{waifu.DisplayMultModificator(statModifier)}})</span>
        
    </div>
</template>

<style lang="css" scoped>

.shortStat, .stat {
    display: grid;
    grid-template-columns: 2.53fr 1.5fr 1fr 1fr;
}

.shortStat
{
    grid-template-columns: 1fr 1fr;
}

.modifier {
    width:60px;
}
</style>