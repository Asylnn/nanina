<script lang="ts">
import ClientResponseType from '@/classes/client_response_type';
import type Item from '@/classes/item/item';
import ItemType from '@/classes/item/item_type';
import StatModifier from '@/classes/modifiers/stat_modifier';
import User from '@/classes/user/user';


export default {
    name : "ItemUse",
    data(){
        return {
            ItemType : ItemType,
            StatModifier: StatModifier,
            publicPath : import.meta.env.BASE_URL,
            items : [] as Array<Item>,
        }
    },
    mounted() {
        this.items = this.user.inventory.userConsumable.filter(item => this.itemIds.includes(item.id))
    },
    props: {
        itemIds : {
            type : Array<Number>,
            required: true
        },
        user : {
            type : User,
            required: true
        }
    },
    methods :{
        useItem(item : Item)
        {
            this.SendToServer(ClientResponseType.UseUserConsumable, item.inventoryId.toString(), this.user.Id)
        }
    }
}


</script>
<template>
    <div>
        <div v-for="item in items">
            <div id="focusedObject">
                <div id="itemImage">
                    <div><img class="itemImage" :src="`${publicPath}/item-image/${item.imgPATH}`"></div>
                    <div><button class="smallbutton" @click="useItem(item)"> {{ $t("item.use") }}</button></div>
                </div>
                <div id="ItemInfo">
                    {{ $t(`item.${item.id}.name`) }} ({{ item.count }})<br>
                    {{ $t(`item.${item.id}.description`) }}<br><br>
                    <p>{{$t("modifiers.modifier")}} </p><br>
                    <div v-for="modifier in item.modifiers">
                        <div class="modifier" v-if="modifier.stat == StatModifier.MaxEnergy">
                            <span>{{ $t(`modifiers.${StatModifier.MaxEnergy}.name`) }}</span>
                            <span>{{ user.max_energy }} ➔ <span class="upgrade">{{ user.max_energy + modifier.amount }}</span></span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<style lang="css" scoped>


#focusedObject {
    position: fixed;
    /*min-width: 400px;
    min-height: 100px;
    width: 30vw;
    height: 15vh;*/
    border-radius: 15px;
    display: flex;
    flex-direction: row;
    padding: 2vh 2vh;
    z-index: 727;
    top:50%;                                /*Make the display be at the center of the screen*/
    left:50%;
    transform: translate(-50%, -50%);
    background-color: rgb(6, 16, 26);
    text-align: left;
}

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