<script lang="ts">

export default {
    name : "Notification",
    props: {
        notifType : {
            type : String,
            required : true
        },
        message : {
            type : String,
            required : true
        },
    },
    emits: ["addMeToMenu"],
    methods : {
        onClickChangePage(page: Page){
            this.$emit("page-change", page)

        },
        onClickConnect() {
            this.$emit("connect-change", !this.logged)
        },
    },
}


</script>

<template>
    <div>
        <span>{{ notifType }} : message</span>  


        <!-- <select v-model="selected_theme" @change="onChangeTheme()">
            <option value = "dark_theme">Dark Theme</option>
            <option value = "white_theme">White Theme</option>
            <option value = "cute_theme">Cute Theme</option>
        </select>
        <p>We need your ids in different game for accessing your scores</p>
        <span>osu id : </span><input type="number" placeholder="727" v-model.number.lazy="entered_id"> COUCOU!!!!
        <button @click="updateSettings()">update</button> -->
    </div>
</template>

<style lang="css" scoped>

</style>