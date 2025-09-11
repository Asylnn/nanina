<script lang="ts">
import ClientResponseType from '@/classes/client_response_type';
import type Item from '@/classes/item/item';
import ItemType from '@/classes/item/item_type';
import StatModifier from '@/classes/modifiers/stat_modifier';
import User from '@/classes/user/user';
import DisplayComponent from './DisplayComponent.vue';


export default {
    name : "CherishMenu",
    data(){
        return {
            ItemType : ItemType,
            StatModifier: StatModifier,
            publicPath : import.meta.env.BASE_URL,
        }
    },
    props: {
        user : {
            type : User,
            required: true
        },
        items: {
            type: Array<Item>,
            required : true,
        }
    },
    methods :{
        useItem(item : Item)
        {
            this.SendToServer(ClientResponseType.UseUserConsumable, item.id.toString(), this.user.Id)
        }
    },
    components:{
        DisplayComponent,
    }
}


</script>
<template>
    <DisplayComponent>
        <div v-for="item in items">
            <div>
                <div id="itemImage">
                    <div><img class="itemImage" :src="`${publicPath}/item-image/${item.imgPATH}`"></div>
                    <div><button class="smallbutton" @click="useItem(item)"> {{ $t("item.use") }}</button></div>
                </div>
                <div id="ItemInfo">
                    {{ $t(`item.${item.id}.name`) }} ({{ user.inventory.items[item.id]?.count || 0}})<br>
                    {{ $t(`item.${item.id}.description`) }}<br><br>
                    <p>{{$t("modifiers.modifier")}} </p><br>
                    <div v-for="modifier in item.modifiers">
                        <div class="modifier" v-if="modifier.stat == StatModifier.MaxEnergy">
                            <span>{{ $t(`modifiers.${StatModifier.MaxEnergy}.name`) }}</span>
                            <span>{{ user.max_energy }} ➔ <span class="upgrade">{{ user.max_energy + modifier.amount }}</span></span>
                        </div>
                        <div class="modifier" v-if="modifier.stat == StatModifier.EnergyRegen">
                            <span>{{ $t(`modifiers.${StatModifier.EnergyRegen}.name`) }}</span>
                            <span>{{ user.energy }} ➔ <span class="upgrade">{{ user.energy + modifier.amount }}</span></span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </DisplayComponent>
</template>

<style lang="css" scoped>

.itemImage {
    /*display: flex;
    flex-direction: row;
    max-width: 25vw;
    max-height: 60vh;*/
    padding-right: 1vw;
    height: 100px;
    width: 100px;
}

.modifier {
    display: grid;
    grid-template-columns: 4fr 1fr;
}

.upgrade
{
    color:green
}

button
{
    margin-top: 10px;
    margin-left: 1vw;
}
</style>