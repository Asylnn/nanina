<script lang="ts">

export default {
    name:"UserOptionPage",
    data() {
        return {
            selected_theme :this.theme,
            entered_id :727,
        }
    },
    props:{
        theme : {
            type : String, 
            required : true,
        },
        id : {
            type : String,
            required : true,
        }
    },
    methods : {
        onChangeTheme(){
            this.$emit("theme-change", this.selected_theme)
        },
        updateSettings(){
            //@ts-ignore
			this.ws.send(JSON.stringify({type:"update osu id", data:this.entered_id, id: this.id}))
        }
    }
}

</script>



<template>
    <div>
        <select v-model="selected_theme" @change="onChangeTheme()">
            <option value = "dark_theme">Dark Theme</option>
            <option value = "white_theme">White Theme</option>
            <option value = "cute_theme">Cute Theme</option>
        </select>
        <p>We need your ids in different game for accessing your scores</p>
        <span>osu id : </span><input type="number" placeholder="727" v-model.number.lazy="entered_id">
        <button @click="updateSettings()">update</button>
    </div>
</template>