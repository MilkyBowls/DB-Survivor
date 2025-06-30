
local toolBar = FF.toolBar("main")
toolBar.iconSize = 32
toolBar:addButton("file_new")
toolBar:addButton("file_close") 
toolBar:addButton("file_open") 
toolBar:addButton("file_save") 
toolBar:addSeparator() 
toolBar:addButton("file_run") 
toolBar:addButton("debug_start_engine")
toolBar:addSeparator() 
toolBar:addButton("editor_def") 
toolBar:addButton("editor_spr") 
toolBar:addButton("editor_anim")
toolBar:addButton("editor_bg")
toolBar:addButton("editor_cmd") 
toolBar:addButton("editor_st") 
toolBar:addButton("editor_snd") 
toolBar:addButton("ref_docs") 
toolBar:addSeparator() 
toolBar:addButton("tools_options") 

local panel = FF.panel("definitions")
local layout = FF.verticalLayout(panel)
layout.margin = 8
layout:addControl("def_tree")

panel = FF.panel("sprites") 
layout = FF.gridLayout(panel)
layout.margin = 8
layout.spacing = 4

layout:addControl("spr_sel", 0, 0, 1, -1):horizontal()
layout:addControl("spr_sel_lb", 1, 0, 1, -1):align(FF.Align.HCenter, FF.Align.VCenter) 

local layout2 = FF.gridLayout()
layout2.spacing = 4
layout2:addControl("spr_name_lb", 0, 0, 1, -1)
layout2:addControl("spr_name", 1, 0, 1, -1)
layout2:addControl("spr_grp_lb", 2, 0, 1, 2)
layout2:addControl("spr_grp", 3, 0, 1, 2)
layout2:addControl("spr_num_lb", 2, 2, 1, 2)
layout2:addControl("spr_num", 3, 2, 1, 2) 
layout2:addControl("spr_xaxis_lb", 4, 0, 1, 2)
layout2:addControl("spr_xaxis", 5, 0, 1, 2)
layout2:addControl("spr_yaxis_lb", 4, 2, 1, 2)
layout2:addControl("spr_yaxis", 5, 2, 1, 2) 
layout:addLayout(layout2, 2, 0, 1, -1)

layout:addControl("spr_dec_load", 3, 0, 1, -1) 
layout:addControl("spr_trans_color", 5, 0, 1, -1) 
layout:addControl("spr_info_lb", 6, 0, 1, -1):align(FF.Align.HCenter, FF.Align.VCenter) 
layout:addControl("spr_onion_skin", 7, 0, 1, -1) 
layout:addControl("spr_onion_sel", 8, 0, 1, -1):horizontal() 
layout:addControl("spr_onion_lb", 9, 0, 1, -1):align(FF.Align.HCenter, FF.Align.VCenter)

layout:addControl("spr_layers_menu", 2, 0):icon(22):flat() 
layout:addControl("spr_layers_blendmode", 2, 1, 1, 2)
layout:addControl("spr_layers_opacity", 2, 3)
layout:addControl("spr_layers_list", 3, 0, 11, 4)
layout:addControl("spr_tool_canvasmove", 2, 4):icon(22):flat() 
layout:addControl("spr_tool_selmove", 3, 4):icon(22):flat()
layout:addControl("spr_tool_rectsel", 4, 4):icon(22):flat()
layout:addControl("spr_tool_freesel", 5, 4):icon(22):flat()
layout:addControl("spr_tool_magicwand", 6, 4):icon(22):flat()
layout:addControl("spr_tool_picker", 7, 4):icon(22):flat()
layout:addControl("spr_tool_pen", 8, 4):icon(22):flat()
layout:addControl("spr_tool_eraser", 9, 4):icon(22):flat()
layout:addControl("spr_tool_remap_pen", 10, 4):icon(22):flat()
layout:addControl("spr_tool_paintbucket", 11, 4):icon(22):flat()
layout:addControl("spr_foreground", 12, 4):size(26,26) 
layout:addControl("spr_background", 13, 4):size(26,26) 

layout:addVerticalSpacer(14, 0)

panel = FF.extraPanel("palettes") 
layout = FF.gridLayout(panel)
layout.margin = 8
layout.spacing = 6

layout:addControl("pal_sel", 0, 0, 1, -1):horizontal()
layout:addControl("pal_sel_lb", 1, 0, 1, -1):align(FF.Align.HCenter, FF.Align.VCenter) 
layout:addControl("spr_switch_pal", 2, 0, 1, -1) 
layout:addControl("pal_grp_lb", 3, 0)
layout:addControl("pal_grp", 4, 0)
layout:addControl("pal_num_lb", 3, 1)
layout:addControl("pal_num", 4, 1)
layout:addControl("pal_view", 5, 0, 1, -1) 
layout:addVerticalSpacer(6, 0)

panel = FF.panel("animations") 
layout = FF.gridLayout(panel)
layout.margin = 8
layout.spacing = 4

layout:addControl("anim_sel", 0, 0, 1, -1):horizontal()
layout:addControl("anim_sel_lb", 1, 0, 1, -1):align(FF.Align.HCenter, FF.Align.VCenter) 

layout2 = FF.gridLayout()
layout2.spacing = 4
layout2:addControl("anim_number_lb", 0, 0) 
layout2:addControl("anim_number", 1, 0) 
layout2:addControl("anim_name_lb", 0, 1, 1, 3) 
layout2:addControl("anim_name", 1, 1, 1, 3) 
layout:addLayout(layout2, 2, 0, 2, -1)

layout:addControl("anim_frame_lb", 4, 0, 1, -1):align(FF.Align.HCenter, FF.Align.VCenter) 
layout:addControl("anim_frame_sel", 5, 0, 1, -1):horizontal()
layout:addControl("anim_frame_sel_lb", 6, 0, 1, -1):align(FF.Align.HCenter, FF.Align.VCenter) 
layout:addControl("anim_frame_grp_lb", 7, 0, 1, 2) 
layout:addControl("anim_frame_grp", 8, 0, 1, 2)
layout:addControl("anim_frame_num_lb", 7, 2, 1, 2)  
layout:addControl("anim_frame_num", 8, 2, 1, 2) 
layout:addControl("anim_frame_xaxis_lb", 9, 0, 1, 2) 
layout:addControl("anim_frame_xaxis", 10, 0, 1, 2)  
layout:addControl("anim_frame_yaxis_lb", 9, 2, 1, 2) 
layout:addControl("anim_frame_yaxis", 10, 2, 1, 2)  
layout:addControl("anim_frame_time_lb", 11, 0, 1, 2) 
layout:addControl("anim_frame_time", 12, 0, 1, 2)  
layout:addControl("anim_frame_flip_lb", 11, 2, 1, 2) 
layout:addControl("anim_frame_flip", 12, 2, 1, 2)  
layout:addControl("anim_frame_trans_lb", 13, 0, 1, 2) 
layout:addControl("anim_frame_trans", 14, 0, 1, 2) 
layout:addControl("anim_frame_transsrc_lb", 13, 2) 
layout:addControl("anim_frame_transsrc", 14, 2) 
layout:addControl("anim_frame_transdst_lb", 13, 3) 
layout:addControl("anim_frame_transdst", 14, 3) 

layout2 = FF.gridLayout()
layout2.spacing = 4
layout2:addControl("anim_frame_xscale_lb", 0, 0) 
layout2:addControl("anim_frame_xscale", 1, 0) 
layout2:addControl("anim_frame_yscale_lb", 0, 1) 
layout2:addControl("anim_frame_yscale", 1, 1) 
layout2:addControl("anim_frame_angle_lb", 0, 2) 
layout2:addControl("anim_frame_angle", 1, 2) 
layout:addLayout(layout2, 15, 0, 1, -1)

layout:addControl("anim_frame_loop", 16, 0, 1, -1) 
layout:addControl("anim_frame_intp", 17, 0, 1, -1):align(FF.Align.HCenter, FF.Align.VCenter) 
layout:addControl("anim_frame_intp_blend", 18, 0, 1, 2) 
layout:addControl("anim_frame_intp_offset", 18, 2, 1, 2) 
layout:addControl("anim_frame_intp_scale", 19, 0, 1, 2) 
layout:addControl("anim_frame_intp_angle", 19, 2, 1, 2) 

layout2 = FF.gridLayout()
layout2.spacing = 8
layout2:addControl("anim_onion_skin", 0, 0, 1, -1) 
layout2:addControl("anim_onion_sel", 1, 0, 1, 7):horizontal()
layout2:addControl("anim_onion_find", 1, 7)
layout2:addControl("anim_onion_framesel", 2, 0, 1, -1):horizontal()
layout2:addControl("anim_onion_lb", 3, 0, 1, -1):align(FF.Align.HCenter, FF.Align.VCenter)
layout:addLayout(layout2, 20, 0, 1, -1)

layout:addVerticalSpacer(21, 0)

panel = FF.extraPanel("thumbnails") 
layout = FF.verticalLayout(panel)
layout.margin = 8
layout.spacing = 6
layout:addControl("thumb_grp_list") 
layout:addControl("thumb_view") 

panel = FF.panel("commands") 
layout = FF.verticalLayout(panel)
layout.margin = 8
layout:addControl("cmd_tree") 

panel = FF.panel("states") 
layout = FF.verticalLayout(panel)
layout.margin = 8
layout.spacing = 4
layout:addControl("st_tree") 

panel = FF.panel("sounds") 
layout = FF.gridLayout(panel)
layout.margin = 8
layout.spacing = 4

layout:addControl("snd_sel", 1, 0, 1, -1):horizontal()
layout:addControl("snd_sel_lb", 2, 0, 1, -1):align(FF.Align.HCenter, FF.Align.VCenter)
layout:addControl("snd_grp_lb", 3, 0, 1, 4) 
layout:addControl("snd_grp", 4, 0, 1, 4) 
layout:addControl("snd_num_lb", 3, 4, 1, 4) 
layout:addControl("snd_num", 4, 4, 1, 4) 
layout:addControl("snd_info_lb", 5, 0, 1, 7) 
layout:addControl("snd_volume", 5, 7):vertical() 

layout:addControl("snd_curr_sample_lb", 6, 0, 1, 3) 
layout:addControl("snd_curr_sample", 6, 3, 1, 3) 
layout:addControl("snd_save_changes", 6, 6):icon(22):flat()
layout:addControl("snd_discard_changes", 6, 7):icon(22):flat()

layout2 = FF.gridLayout()
layout2.margin = 8
layout2.spacing = 4
layout2:addControl("snd_start_lb", 0, 0) 
layout2:addControl("snd_end_lb", 0, 1)
layout2:addControl("snd_sel_start", 1, 0) 
layout2:addControl("snd_sel_end", 1, 1) 
layout:addControl("snd_selection", 7, 0, 1, -1):layout(layout2)

layout:addVerticalSpacer(8, 0)

panel = FF.panel("backgrounds") 
layout = FF.gridLayout(panel)
layout.margin = 8
layout.spacing = 4

layout:addControl("bgs_sel", 1, 0, 1, -1):horizontal()
layout:addControl("bgs_sel_lb", 2, 0, 1, -1):align(FF.Align.HCenter, FF.Align.VCenter) 
layout:addControl("bgs_name_lb", 3, 0, 1, 5) 
layout:addControl("bgs_name", 4, 0, 1, 5) 
layout:addControl("bgs_type_lb", 3, 5, 1, 3) 
layout:addControl("bgs_type", 4, 5, 1, 3) 

layout2 = FF.horizontalLayout()
layout2.margin = 0
layout2.spacing = 4

local layout3 = FF.gridLayout()
layout3.margin = 8
layout3.spacing = 4
layout3:addControl("bgs_grp_lb", 0, 0) 
layout3:addControl("bgs_spr_grp", 0, 1)
layout3:addControl("bgs_num_lb", 1, 0) 
layout3:addControl("bgs_spr_num", 1, 1) 
layout2:addControl("bgs_spr"):layout(layout3)

layout3 = FF.verticalLayout()
layout3.margin = 8
layout3.spacing = 4
layout3:addControl("bgs_anim_no_lb") 
layout3:addControl("bgs_anim_no")
layout3:addControl("bgs_mask") 
layout2:addLayout(layout3, 0)

layout:addLayout(layout2, 5, 0, 1, -1)

layout2 = FF.horizontalLayout()
layout2.margin = 0
layout2.spacing = 4

layout3 = FF.gridLayout()
layout3.margin = 8
layout3.spacing = 4
layout3:addControl("bgs_x_lb", 0, 0) 
layout3:addControl("bgs_start_x", 0, 1)
layout3:addControl("bgs_y_lb", 1, 0) 
layout3:addControl("bgs_start_y", 1, 1) 
layout2:addControl("bgs_start"):layout(layout3)

layout3 = FF.gridLayout()
layout3.margin = 8
layout3.spacing = 4
layout3:addControl("bgs_x_lb", 0, 0) 
layout3:addControl("bgs_delta_x", 0, 1)
layout3:addControl("bgs_y_lb", 1, 0) 
layout3:addControl("bgs_delta_y", 1, 1) 
layout2:addControl("bgs_delta"):layout(layout3)

layout:addLayout(layout2, 6, 0, 1, -1)

layout2 = FF.horizontalLayout()
layout2.margin = 0
layout2.spacing = 4

layout3 = FF.gridLayout()
layout3.margin = 8
layout3.spacing = 4
layout3:addControl("bgs_x_lb", 0, 0) 
layout3:addControl("bgs_velocity_x", 0, 1)
layout3:addControl("bgs_y_lb", 1, 0) 
layout3:addControl("bgs_velocity_y", 1, 1) 
layout2:addControl("bgs_velocity"):layout(layout3)

layout3 = FF.gridLayout()
layout3.margin = 0
layout3.spacing = 4
layout3:addControl("bgs_id_lb", 0, 0) 
layout3:addControl("bgs_id", 0, 1) 
layout3:addControl("bgs_layer_lb", 1, 0) 
layout3:addControl("bgs_layer", 1, 1)
layout3:addControl("bgs_pos_link", 2, 0, 1, -1) 
layout2:addLayout(layout3, 0)

layout:addLayout(layout2, 7, 0, 1, -1)

layout2 = FF.horizontalLayout()
layout2.margin = 0
layout2.spacing = 4

layout3 = FF.gridLayout()
layout3.margin = 8
layout3.spacing = 4
layout3:addControl("bgs_x_lb", 0, 0) 
layout3:addControl("bgs_tile_x", 0, 1)
layout3:addControl("bgs_y_lb", 1, 0) 
layout3:addControl("bgs_tile_y", 1, 1) 
layout2:addControl("bgs_tile"):layout(layout3)

layout3 = FF.gridLayout()
layout3.margin = 8
layout3.spacing = 4
layout3:addControl("bgs_x_lb", 0, 0) 
layout3:addControl("bgs_tile_spacing_x", 0, 1)
layout3:addControl("bgs_y_lb", 1, 0) 
layout3:addControl("bgs_tile_spacing_y", 1, 1) 
layout2:addControl("bgs_tile_spacing"):layout(layout3)

layout:addLayout(layout2, 8, 0, 1, -1)

layout:addControl("bgs_trans_lb", 9, 0, 1, 4) 
layout:addControl("bgs_trans", 10, 0, 1, 4) 
layout:addControl("bgs_transsrc_lb", 9, 4, 1, 2) 
layout:addControl("bgs_transsrc", 10, 4, 1, 2) 
layout:addControl("bgs_transdst_lb", 9, 6, 1, 2) 
layout:addControl("bgs_transdst", 10, 6, 1, 2) 

layout2 = FF.gridLayout()
layout2.margin = 8
layout2.spacing = 4
layout2:addControl("bgs_mode_lb", 0, 0) 
layout2:addControl("bgs_parallax_mode", 0, 1, 1, 3)
layout2:addControl("bgs_top_lb", 1, 0, 1, 2) 
layout2:addControl("bgs_parallax_top", 2, 0, 1, 2)
layout2:addControl("bgs_bottom_lb", 1, 2, 1, 2) 
layout2:addControl("bgs_parallax_bottom", 2, 2, 1, 2) 
layout:addControl("bgs_parallax", 11, 0, 1, -1):layout(layout2)

layout2 = FF.horizontalLayout()
layout2.margin = 0
layout2.spacing = 4

layout3 = FF.gridLayout()
layout3.margin = 8
layout3.spacing = 4
layout3:addControl("bgs_x_lb", 0, 0) 
layout3:addControl("bgs_scale_start_x", 0, 1)
layout3:addControl("bgs_y_lb", 1, 0) 
layout3:addControl("bgs_scale_start_y", 1, 1) 
layout2:addControl("bgs_scale_start"):layout(layout3)

layout3 = FF.gridLayout()
layout3.margin = 8
layout3.spacing = 4
layout3:addControl("bgs_x_lb", 0, 0) 
layout3:addControl("bgs_scale_delta_x", 0, 1)
layout3:addControl("bgs_y_lb", 1, 0) 
layout3:addControl("bgs_scale_delta_y", 1, 1) 
layout2:addControl("bgs_scale_delta"):layout(layout3)

layout:addLayout(layout2, 12, 0, 1, -1)

layout2 = FF.horizontalLayout()
layout2.margin = 0
layout2.spacing = 4

layout3 = FF.gridLayout()
layout3.margin = 8
layout3.spacing = 4
layout3:addControl("bgs_start_lb", 0, 0) 
layout3:addControl("bgs_y_scale_start", 0, 1)
layout3:addControl("bgs_delta_lb", 1, 0) 
layout3:addControl("bgs_y_scale_delta", 1, 1) 
layout2:addControl("bgs_y_scale"):layout(layout3)

layout3 = FF.gridLayout()
layout3.margin = 8
layout3.spacing = 4
layout3:addControl("bgs_x_lb", 0, 0) 
layout3:addControl("bgs_window_delta_x", 0, 1)
layout3:addControl("bgs_y_lb", 1, 0) 
layout3:addControl("bgs_window_delta_y", 1, 1) 
layout2:addControl("bgs_window_delta"):layout(layout3)

layout:addLayout(layout2, 13, 0, 1, -1)

layout3 = FF.gridLayout()
layout3.margin = 8
layout3.spacing = 4
layout3:addControl("bgs_mode_lb", 0, 0) 
layout3:addControl("bgs_window_mode", 0, 1, 1, 3)
layout3:addControl("bgs_top_lb", 1, 0) 
layout3:addControl("bgs_window_top", 2, 0)
layout3:addControl("bgs_left_lb", 1, 1) 
layout3:addControl("bgs_window_left", 2, 1) 
layout3:addControl("bgs_bottom_lb", 1, 2) 
layout3:addControl("bgs_window_bottom", 2, 2)
layout3:addControl("bgs_right_lb", 1, 3) 
layout3:addControl("bgs_window_right", 2, 3)
layout:addControl("bgs_window", 14, 0, 1, -1):layout(layout3)

layout3 = FF.gridLayout()
layout3.margin = 8
layout3.spacing = 4
layout3:addControl("bgs_sin_x_ampl_lb", 0, 0) 
layout3:addControl("bgs_sin_x_ampl", 1, 0)
layout3:addControl("bgs_sin_x_period_lb", 0, 1) 
layout3:addControl("bgs_sin_x_period", 1, 1) 
layout3:addControl("bgs_sin_x_offset_lb", 0, 2) 
layout3:addControl("bgs_sin_x_offset", 1, 2)
layout:addControl("bgs_sin_x", 15, 0, 1, -1):layout(layout3)

layout3 = FF.gridLayout()
layout3.margin = 8
layout3.spacing = 4
layout3:addControl("bgs_sin_y_ampl_lb", 0, 0) 
layout3:addControl("bgs_sin_y_ampl", 1, 0)
layout3:addControl("bgs_sin_y_period_lb", 0, 1) 
layout3:addControl("bgs_sin_y_period", 1, 1) 
layout3:addControl("bgs_sin_y_offset_lb", 0, 2) 
layout3:addControl("bgs_sin_y_offset", 1, 2)
layout:addControl("bgs_sin_y", 16, 0, 1, -1):layout(layout3)

layout:addVerticalSpacer(17, 0)

panel = FF.panel("emulator") 
layout = FF.gridLayout(panel)
layout.margin = 8
layout.spacing = 6

layout:addControl("emul_engine_lb", 0, 0, 1, -1)
layout:addControl("emul_engine", 1, 0, 1, -1)

layout2 = FF.gridLayout()
layout2.margin = 8
layout2.spacing = 6
layout2:addControl("emul_match_mode_lb", 0, 0) 
layout2:addControl("emul_match_mode", 1, 0)
layout2:addControl("emul_enemy_lb", 0, 1) 
layout2:addControl("emul_enemy", 1, 1)
layout2:addControl("emul_rounds_lb", 2, 0) 
layout2:addControl("emul_rounds", 3, 0)
layout2:addControl("emul_time_lb", 2, 1) 
layout2:addControl("emul_time", 3, 1)
layout2:addControl("emul_explod_mode_lb", 4, 0, 1, -1) 
layout2:addControl("emul_explod_mode", 5, 0, 1, -1)
layout2:addControl("emul_debug_info", 6, 0, 1, -1)
layout:addControl("emul_match", 2, 0, 1, -1):layout(layout2)

layout2 = FF.gridLayout()
layout2.margin = 8
layout2.spacing = 6
layout2:addControl("emul_dummy_ctrl_lb", 0, 0) 
layout2:addControl("emul_dummy_ctrl", 1, 0)
layout2:addControl("emul_dummy_mode_lb", 0, 1) 
layout2:addControl("emul_dummy_mode", 1, 1)
layout2:addControl("emul_guard_mode_lb", 2, 0) 
layout2:addControl("emul_guard_mode", 3, 0)
layout2:addControl("emul_distance_lb", 2, 1) 
layout2:addControl("emul_distance", 3, 1) 
layout2:addControl("emul_button_jam_lb", 4, 0) 
layout2:addControl("emul_button_jam", 5, 0)
layout2:addControl("emul_ai_level_lb", 4, 1) 
layout2:addControl("emul_ai_level", 5, 1)
layout:addControl("emul_training", 3, 0, 1, -1):layout(layout2)

layout:addControl("emul_screen_lb", 4, 0, 1, -1)
layout:addControl("emul_screen", 5, 0, 1, -1)

layout:addVerticalSpacer(6, 0)

panel = FF.extraPanel("organizer") 
layout = FF.verticalLayout(panel)
layout.margin = 8
layout.spacing = 6

layout2 = FF.gridLayout()
layout2:addControl("org_up", 0, 0):icon(22):flat()
layout2:addControl("org_down", 0, 1):icon(22):flat()
layout2:addControl("org_up_10", 0, 2):icon(22):flat()
layout2:addControl("org_down_10", 0, 3):icon(22):flat()
layout2:addControl("org_top", 0, 4):icon(22):flat()
layout2:addControl("org_bottom", 0, 5):icon(22):flat()
layout2:addControl("org_auto", 0, 6):icon(22):flat()
layout2:addControl("org_palette", 0, 7):icon(22):flat()
layout:addLayout(layout2, 0)

layout:addControl("org_list") 

panel = FF.extraPanel("controllers") 
layout = FF.gridLayout(panel)
layout.margin = 8
layout.spacing = 4

layout2 = FF.horizontalLayout()
--layout2:addControl("bgctrls_add"):icon(22):flat()
--layout2:addControl("bgctrls_delete"):icon(22):flat()
layout:addLayout(layout2, 0, 0, 1, -1)

layout:addControl("bgctrls_def_sel", 1, 0, 1, -1):horizontal()
layout:addControl("bgctrls_def_sel_lb", 2, 0, 1, -1):align(FF.Align.HCenter, FF.Align.VCenter) 
layout:addControl("bgctrls_def_name_lb", 3, 0, 1, 5) 
layout:addControl("bgctrls_def_name", 4, 0, 1, 5) 
layout:addControl("bgctrls_def_loop_lb", 3, 5, 1, 3) 
layout:addControl("bgctrls_def_loop", 4, 5, 1, 3) 

layout3 = FF.gridLayout()
layout3.margin = 8
layout3.spacing = 4
layout3:addControl("bgctrls_def_id_1", 0, 0) 
layout3:addControl("bgctrls_def_id_2", 0, 1)
layout3:addControl("bgctrls_def_id_3", 0, 2) 
layout3:addControl("bgctrls_def_id_4", 0, 3) 
layout3:addControl("bgctrls_def_id_5", 0, 4) 
layout3:addControl("bgctrls_def_id_6", 1, 0) 
layout3:addControl("bgctrls_def_id_7", 1, 1)
layout3:addControl("bgctrls_def_id_8", 1, 2) 
layout3:addControl("bgctrls_def_id_9", 1, 3) 
layout3:addControl("bgctrls_def_id_10", 1, 4) 
layout:addControl("bgctrls_def_ids", 5, 0, 1, -1):layout(layout3)

layout:addControl("bgctrls_sel", 6, 0, 1, -1):horizontal()
layout:addControl("bgctrls_sel_lb", 7, 0, 1, -1):align(FF.Align.HCenter, FF.Align.VCenter) 
layout:addControl("bgctrls_name_lb", 8, 0, 1, 4) 
layout:addControl("bgctrls_name", 9, 0, 1, 4) 
layout:addControl("bgctrls_type_lb", 8, 4, 1, 4) 
layout:addControl("bgctrls_type", 9, 4, 1, 4) 

layout3 = FF.gridLayout()
layout3.margin = 8
layout3.spacing = 4
layout3:addControl("bgctrls_start_lb", 0, 0) 
layout3:addControl("bgctrls_start_time", 1, 0)
layout3:addControl("bgctrls_end_lb", 0, 1) 
layout3:addControl("bgctrls_end_time", 1, 1) 
layout3:addControl("bgctrls_loop_lb", 0, 2) 
layout3:addControl("bgctrls_loop_time", 1, 2)
layout:addControl("bgctrls_time", 10, 0, 1, -1):layout(layout3)

layout:addControl("bgctrls_bool", 11, 0, 1, -1) 
layout:addControl("bgctrls_anim_no_lb", 12, 0, 1, 4)
layout:addControl("bgctrls_anim_no", 12, 4, 1, 4)  
layout:addControl("bgctrls_x_lb", 13, 0, 1, 1) 
layout:addControl("bgctrls_x", 13, 1, 1, 3) 
layout:addControl("bgctrls_y_lb", 13, 4, 1, 1) 
layout:addControl("bgctrls_y", 13, 5, 1, 3) 

layout3 = FF.gridLayout()
layout3.margin = 8
layout3.spacing = 4
layout3:addControl("bgctrls_ampl_lb", 0, 0) 
layout3:addControl("bgctrls_ampl", 1, 0)
layout3:addControl("bgctrls_period_lb", 0, 1) 
layout3:addControl("bgctrls_period", 1, 1) 
layout3:addControl("bgctrls_offset_lb", 0, 2) 
layout3:addControl("bgctrls_offset", 1, 2)
layout:addLayout(layout3, 14, 0, 1, -1)

layout:addVerticalSpacer(15, 0)

panel = FF.extraPanel("debug") 
layout = FF.verticalLayout(panel)
layout.margin = 8
layout.spacing = 6

layout:addControl("debug_variables") 
