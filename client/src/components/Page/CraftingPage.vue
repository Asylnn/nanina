<script lang="ts">
import Craft from '@/classes/crafting/craft';
import type CraftIngrendient from '@/classes/crafting/craft_ingredient';
import type Item from '@/classes/item/item';
import User from '@/classes/user/user';
import ActivityProgressComponent from '../Component/ActivityProgressComponent.vue';
import ActivityWaifuPickerComponent from '../Component/ActivityWaifuPickerComponent.vue';
import ActivityType from '@/classes/user/activity_type';
import Waifu from '@/classes/waifu/waifu';


export default {
    name : "CraftingPage",
    data() {
        return {
            publicPath: import.meta.env.BASE_URL,
            ActivityType:ActivityType,
            aboveLimits:false,
        }
    },
    props: {
        item_db:{
            type:Array<Item>,
            required: true,
        },
        user:{
            type:User,
            required:true,
        },
        craftingRecipes:{
            type:Array<Craft>,
            required:true,
        },
        selectedWaifu:{
            type:[Waifu, null],
            required:true,
        }
    },
    methods: {
        getImgUrl(id: number)
        {
            return this.item_db.find(itemdb => itemdb.id == id)!.imgPATH
        },
        getColor(qty : number)
        {
            return "color:" +  (qty >= 1 ? "purple" : "")
        },
        isAboveLimit(qty1 : number, qty2 : number)
        {
            return "color:" + (qty1 > qty2 ? "red" : "")
        },
        sendWaifuToCrafting()
        {
            let craftingList: {id: number, quantity: number}[] = []
            
            //Deep copy of the poor!
            let craftingRequest = JSON.parse(JSON.stringify(this.craftingRecipes.filter(craft => craft.quantity >= 1))) as Craft[]
            craftingRequest.forEach(cr => {
                craftingList.push({
                    id : cr.id,
                    quantity : cr.quantity,
                })
            })

            let craftingRequesttoClient = {
                craftingList: craftingList,
                waifuID: this.selectedWaifu!.id,
                activityType: ActivityType.Crafting,
            }
            this.SendToServer("send waifu to activity", JSON.stringify(craftingRequesttoClient), this.user.Id)
            this.$emit("reset-selected-waifu")
            this.craftingRecipes.forEach(craft => {
                craft.quantity = 0;
            })
        },
        getItemQuantity(id: number) : number
        {
            let item = this.user.inventory.material.find(item => item.id == id)
            return item != undefined ? item!.count : 0 
        },
    },
    computed:
    {
        craftingList()
        {
            this.aboveLimits = false

            this.craftingRecipes.forEach(craft => {
                if(craft.quantity < 0) craft.quantity = 0;
            })

            //Deep copy of the poor!
            let craftingRequest = JSON.parse(JSON.stringify(this.craftingRecipes.filter(craft => craft.quantity >= 1))) as Craft[]
            
            
            /*Each ingredient and result quantity is multiplied by the craft quantity, so we don't have to check craft quantity anymore*/
            craftingRequest.forEach(craft => {
                
                craft.ingredients.forEach(ingredient => ingredient.quantity *= craft.quantity)
                craft.results.forEach(result => result.quantity *= craft.quantity)
            })

            /*We create a craft object which is used as a unique craft that recapitulate all crafts*/
            let craftingList = new Craft()

            /*We join all the ingredients of differents craft into a single ingredients array, same for results*/
            craftingList.ingredients = craftingRequest.reduce((ingredients, craft) => ingredients.concat(craft.ingredients), [] as CraftIngrendient[])
            craftingList.results = craftingRequest.reduce((result, craft) => result.concat(craft.results), [] as CraftIngrendient[])

            /*We remove items with the same id, and add their quantity together*/
            for(let i = 0; i < craftingList.ingredients.length; i++)
            {
                for(let j = i+1; j < craftingList.ingredients.length; j++)
                {
                    if(craftingList.ingredients[i].id == craftingList.ingredients[j].id)
                    {
                        console.log(craftingList)
                        craftingList.ingredients[i].quantity += craftingList.ingredients[j].quantity
                        craftingList.ingredients.splice(j, 1)
                        j--
                    }
                }
                this.aboveLimits ||= craftingList.ingredients[i].quantity > this.getItemQuantity(craftingList.ingredients[i].id)
            }
            for(let i = 0; i < craftingList.results.length; i++)
            {
                for(let j = i+1; j < craftingList.results.length; j++)
                {
                    if(craftingList.results[i].id == craftingList.results[j].id)
                    {
                        console.log(craftingList)
                        craftingList.results[i].quantity += craftingList.results[j].quantity
                        craftingList.results.splice(j, 1)
                        j--
                    }
                }
            }

            /*Finally, we compute the final time and money cost*/
            craftingList.moneyCost = craftingRequest.reduce((cost, craft) => cost + craft.moneyCost*craft.quantity, 0)
            this.aboveLimits ||= craftingList.moneyCost > this.user.money
            craftingList.timeCost = craftingRequest.reduce((cost, craft) => cost + craft.timeCost*craft.quantity, 0)

            return craftingList
        }
    },
    components:
    {
        ActivityProgressComponent,
        ActivityWaifuPickerComponent,
    },
    emits:["show-waifu-selector", "reset-selected-waifu"],
}


</script>

<template>
    <div>
        <div v-for="activity in user.activities.filter(activity => activity.type == ActivityType.Crafting)">
            <ActivityProgressComponent :user="user" :activity="activity">

            </ActivityProgressComponent>
        </div>
        <div class="craftingRecap">
            
            <div v-if="craftingList.results.length != 0">
                <p>{{ $t("activities.crafting.queue")}}</p>
                <div class="craftingMenu flex">
                    <div v-for="ingredient in craftingList.ingredients" class="itemSlot">
                        <div class="itemImage">
                            <img :src="`${publicPath}item-image/${getImgUrl(ingredient.id)}`">
                        </div>
                        <div class="quantity"
                            :style="isAboveLimit(ingredient.quantity, getItemQuantity(ingredient.id))">
                            {{ ingredient.quantity}} / {{getItemQuantity(ingredient.id) }} 
                        </div>
                    </div>
                    ➔
                    <div v-for="result in craftingList.results" class="itemSlot">
                        <div class="itemImage">
                            <img :src="`${publicPath}item-image/${getImgUrl(result.id)}`">
                            
                        </div>
                        <div class="quantity">{{ result.quantity }}</div>
                    </div>
                    <div class="costDisplay flex">
                        <span :style="isAboveLimit(craftingList.moneyCost, user.money)">
                            {{ craftingList.moneyCost }} / {{ user.money }} 
                        </span>
                        <span >{{ craftingList.timeCost }}</span>
                    </div>
                </div>
                <ActivityWaifuPickerComponent v-if="!aboveLimits" :user="user" :selected-waifu="selectedWaifu" :activity-type="ActivityType.Crafting" 
                    v-on:reset-selected-waifu="$emit('reset-selected-waifu')" 
                    v-on:show-waifu-selector="$emit('show-waifu-selector')"
                    v-on:start-crafting-activity="sendWaifuToCrafting()">
                </ActivityWaifuPickerComponent>
            </div>
        </div>
        <div v-for="craft in craftingRecipes" class="craftingMenu flex">
            <div class="craftRecipe flex">
                <div v-for="ingredient in craft.ingredients" class="itemSlot">
                    <div class="itemImage">
                        <img :src="`${publicPath}item-image/${getImgUrl(ingredient.id)}`">
                    </div>
                    <div class="quantity" :style="getColor(craft.quantity)">{{ ingredient.quantity * Math.max(1, craft.quantity)}}</div>
                </div>
                ➔
                <div v-for="result in craft.results" class="itemSlot">
                    <div class="itemImage">
                        <img :src="`${publicPath}item-image/${getImgUrl(result.id)}`">
                    </div>
                    <div class="quantity" :style="getColor(craft.quantity)">{{ result.quantity * Math.max(1, craft.quantity)}}</div>
                </div>
            </div>
            <div class="costDisplay flex">
                <span :style="getColor(craft.quantity)">{{ craft.moneyCost * Math.max(1, craft.quantity)}}</span>
                <span :style="getColor(craft.quantity)">{{ craft.timeCost * Math.max(1, craft.quantity)}}</span>
            </div>
            <div class="quantityPicker flex">
                <button @click="craft.quantity++">▲</button>
                <input type="number" placeholder="0" v-model.number.lazy="craft.quantity"></input>
                <button @click="craft.quantity--">▼</button>
            </div>
        </div>
    </div>
</template>

<style lang="css" scoped>

input
{
    border:none;
    width:20px;
}

.craftingMenu, .craftRecipe
{
    flex-direction: row;
    place-items: center;
    
}

.craftingMenu
{
    margin-bottom: 20px;
}

.craftRecipe
{
    min-width: 30vw;
}

.quantityPicker
{
    width:50px;
    place-items: center;
}

.quantityPicker input
{
    width:50px;
    text-align: center;
}

.costDisplay
{
    min-width: 100px;
    padding: 0px 20px;
}

.itemImage{
    margin :10px;
    border: 6px;
    border-style: solid;
    border-radius: 20px;
    border-color:rgb(20,20,20);
    cursor: pointer;
}

.itemImage img {
    width: 64px;
    height: 64px;
}

.itemSlot span
{
    text-align: center;
}

.quantity
{
    width:96px;
    position: absolute;
    text-align: center;
}

.craftingRecap
{
    min-height: 360px;
}

</style>