<script lang="ts">
import Modifier from '@/classes/modifiers/modifiers';
import StatModifier from '@/classes/modifiers/stat_modifier';


export default {
    name : "ModifierComponent",
    data() {
        return {
            StatModifier: StatModifier,
        }
    },
    props: {
        modifier: {
            type : Modifier,
            required : true
        },
        upgradeQuantity: {
            type: Number,
            default:0,
            required : false,
        }
    },
}

</script>

<template>
    <div>
        <div class="modifier">
            <span>{{ $t(`modifiers.${modifier.stat}.name`) }}</span>
            <span v-if="modifier.operationType==1 || modifier.stat == StatModifier.CritChance || modifier.stat == StatModifier.CritDamage">
                <span v-if="upgradeQuantity == 0" class="stat">+{{ Math.trunc(((modifier.amount)*100)*10)/10}}%</span>
                <span v-else>+{{ Math.trunc(((modifier.amount)*100)*10)/10}}% ➔ <span class="upgrade">+{{ Math.trunc(((modifier.amount + upgradeQuantity)*100)*10)/10}}%</span></span>
            </span>
            <span v-else>
                <span v-if="upgradeQuantity == 0" class="stat">+{{ Math.trunc(modifier.amount)}}</span>
                <span v-else>{{ Math.trunc(modifier.amount) }} ➔ <span class="upgrade">{{ Math.trunc(modifier.amount + upgradeQuantity) }}</span></span>
            </span>
        </div>
    </div>
</template>

<style lang="css" scoped>

.modifier {
    display: grid;
    grid-template-columns: 5fr 1fr;
}

.stat {
    width:60px;
}

.upgrade
{
    color:green
}


</style>