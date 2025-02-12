<script lang="ts">

export default {
    name:"AddMap",
    data() {
        return {
            entered_id: 727
        }
    },
    props:{
        id : {
            type : String,
            required : true,
        }
    },
    methods : {
        updateSettings(){
            this.SendToServer("add beatmap with id", this.entered_id.toString(), this.id)

        }
    }
}

</script>



<template>
    <div>
        <p>Enter Osu Beatmap Id to add it to the database</p>
        <span>Beatmap Id </span><input type="number" placeholder="727" v-model.number.lazy="entered_id">
        <button @click="updateSettings()">add</button>
    </div>
</template>

<style>
input {
    width: 120px;
}
</style>