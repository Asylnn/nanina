<script lang="ts">

export default {
    name : "Theme",
    data() {
        return {
            _theme: this.theme,
        }
    },
    props: {
        theme : {
            type : String,
            required : true
        },
    },
    emits: ["theme-change"],
    methods: {
        onChangeTheme() {
            this.$emit("theme-change", this._theme);
        }
    }
}

</script>

<template>
    <select v-model="_theme" @change="onChangeTheme()">
        <option value = "dark_theme">Dark Theme</option>
        <option value = "white_theme">White Theme</option>
        <option value = "cute_theme">Cute Theme</option>
    </select>
</template>

<style lang="css" scoped>
select {
    padding: 0%;
    width: 100%;
    height: 100%;
    border: 0;
}
</style>