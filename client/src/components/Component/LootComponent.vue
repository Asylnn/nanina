<script lang="ts">
import type Loot from '@/classes/loot/loot';
import LootType from '@/classes/loot/loot_type';
import ItemComponent from './ItemComponent.vue';
import { getRarityStyle, MillisecondsToHourMinuteSecondFormat } from '@/classes/utils';

export default {
    name : "Homepage",
    data() {
        return {
            LootType: LootType,
            publicPath : import.meta.env.BASE_URL,
            getRarityStyle:getRarityStyle,
            MillisecondsToHourMinuteSecondFormat:MillisecondsToHourMinuteSecondFormat,
            /*isDisplayingLoot : false,
            displayedLoot : null as Loot | null*/
        }
    },
    props:{
        loots:{
            type: Array<Loot>,
            required:true,
        },
        playAnimation:{
            type:Boolean,
            required:false,
            default:false,
        }
    },
    emits:["display-loot"],
    components:{
        ItemComponent
    },
    mounted()
    {
        console.log(this.loots)
        //we combine the loot containing the same item id
        for(let i = 0; i < this.loots.length; i++)
        {
            for(let j = i+1; j < this.loots.length; j++)
            {
                if(this.loots[i].item != null && this.loots[i].lootType != LootType.Equipment && ( this.loots[i].item!.id == this.loots[j].item!.id ) )
                {
                    let itemIAmount = this.loots[i].item!.count == 1 ? this.loots[i].amount : this.loots[i].item!.count
                    let itemJAmount = this.loots[j].item!.count == 1 ? this.loots[j].amount : this.loots[j].item!.count
                    this.loots[i].amount = itemIAmount + itemJAmount
                    this.loots.splice(j, 1)
                    j--
                }
            }
        }
        console.log(this.loots)
    }
        

}


</script>
<template>
    <div id="loots" :class="playAnimation ? 'playAnim' : 'test'">
        <div v-for="loot in loots" class="loot">
            <div v-if="loot.lootType == LootType.WaifuXP">
                <img src="@/assets/waifu_xp.svg">
                <div>{{ loot.amount }}</div>
            </div>
            <div v-else-if="loot.lootType == LootType.UserXP">
                <img src="@/assets/user_xp.svg">
                <div>{{ loot.amount }}</div>
            </div>
            <div v-else-if="loot.lootType == LootType.Equipment"  @click="$emit('display-loot', loot)" class="rarityBorder" :style="getRarityStyle(loot.item!.rarity)">
                <img :src="`${publicPath}/item-image/${loot.item!.imgPATH}`">
            </div>
            <div v-else-if="loot.lootType == LootType.Item" c>
                <img :src="`${publicPath}/item-image/${loot.item!.imgPATH}`">
                <div v-if="loot.amount == 1" class="amount">{{ loot.item!.count }}</div>
                <div v-else class="amount">{{ loot.amount }}</div>
            </div>
            <div v-else-if="loot.lootType == LootType.GC" >
                <img src="@/assets/gc.svg">
                <div class="amount">{{ loot.amount }}</div>
            </div>
            <div v-else-if="loot.lootType == LootType.Money">
                <img src="@/assets/ugly_coin.svg">
                <div class="amount">{{ loot.amount }}</div>
            </div>
            <div v-else-if="loot.lootType == LootType.Modifiers">
                <img src="@/assets/unlock.svg">
            </div>
            <div v-else-if="loot.lootType == LootType.TimeSave">
                <img src="@/assets/unlock.svg">
                <div class="amount">{{ MillisecondsToHourMinuteSecondFormat(loot.amount) }}</div>
            </div>
        </div>
        
    </div>
    <div id="square">

        </div>
</template>

<style lang="css" scoped>

img
{
    width: 5vw;
    height: 5vw;
}

.loot
{
    margin-right:20px;
    text-align: center;
    cursor: pointer;
}

.rarityBorder
{
    border-width:5px;
    padding: 8px;
    border-radius:15px;
    border-style:outset;
    width:5vw;
    height:5vw;
}

#loots {
    display: flex;
    padding:10px 20px;
    padding-right: 0px; /*Because loot have margin-right: 20px*/ 
    border-color: grey;
    border-style: outset;
}

/*For the animation*/

.playAnim {
    border-image: linear-gradient(110deg, gray var(--progress), purple var(--progress) calc(var(--progress) + var(--length)),transparent calc(var(--progress) + var(--length)));
    border-image-slice: 1;
    background-position: 200px;
    animation-name: anim;
    animation-duration: 1.2s;
    animation-iteration-count: 1;
}

@property --progress {
    syntax: '<percentage>';
    inherits: false;
    initial-value: 100%;
}

@property --length {
    syntax: '<percentage>';
    inherits: false;
    initial-value: 0%;
}

@keyframes anim {
    0% {
        --progress:0%;
        --length:0%;
        animation-timing-function:linear;
    }
    20% {
        --progress:20%;
        --length:10%;
        animation-timing-function:linear;
    }
    80% {
        --progress:85%;
        --length:10%;
        animation-timing-function:ease-out;
    }
    100% {
        --progress:100%;
        --length:0%;
        
    }
}

</style>