   M S B A M L   `   `   `  ����k  g呼和浩特市精细化天气预报评分系统, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulls>clr-namespace:呼和浩特市精细化天气预报评分系统0呼和浩特市精细化天气预报评分系统  5      I    C呼和浩特市精细化天气预报评分系统.个人评分72h页   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Plocal>clr-namespace:呼和浩特市精细化天气预报评分系统     5        N� Title$  个人评分72h页��5	       � 	Resources 5         contentCenterStyle �� 
TargetType �� Property �� Value  DataGridStyle1&   System.Windows.Controls.DataGrid �� 
TargetType �� Padding J� SnapsToDevicePixels ��Name	 � ColumnDefinitions
 � RowDefinitions   SelectAllCommand �� 
ResourceId �� TypeInTargetAssembly7  1System.Windows.Controls.DataGridHeadersVisibility
   All �� ConverterParameter!   HeadersVisibilityConverter �� 	Converter �� RelativeSource �� AncestorTypeG  ASystem.Windows.Controls.Primitives.DataGridColumnHeadersPresenter  Name   Column ��Name �� CanContentScroll ��Name �� ScrollableHeight � Maximum �� Orientation* �� #ComputedVerticalScrollBarVisibility �� ViewportHeight �� ViewportSize � Value  �� Mode! �� ScrollableWidth," �� %ComputedHorizontalScrollBarVisibility# �� ViewportWidth$ �� Triggers% ]� 
Conditions& �� Property' �� Value��%�  &	        &	 T     �� 5   
   " ��5      .�5      ��  � ��$ Center=�5      5      �� 5   
   "  6'   .�5      ��  � ��# � ��6+   �� 5       � ��# � ��6+   �� 5       � �� �	#FF688CAF6,   �� 5       � ��$ 1q�60   �� 5        �@ RowDetailsVisibilityMode$ VisibleWhenSelected=�69   �� 5       � �� . 6>   �� 5       �@��PanningMode$
 Both=�69   �� 5       �@��IsFlicksEnabled .  67   �� 5       � �� 5      �� 5      "  6&   .R�5      �� #��z��6!   #��z��6M   #��z��6�   # z��6�    . ._�5      �� $ DG_ScrollViewer��6+   |�.  $��13(�6V   ��5   "   �� 5    &   " ��66   .R�5!   *   � 	 5"   .   �� 5#   2   $
��Auto �6C   �� 5$   2   $��* �6C   �� 5%   2   $
��Auto �6C   
 5'   .   �� 5(   2   $
��Auto �6@   �� 5)   2   $��* �6@   �� 5*   2   $
��Auto �6@   .+�5,   .   �� #��Z 65   |�.  ��6v   C� *�� $" DataGridSelectAllButtonStyle��"  +y�6�   �� *HeadersVisibility+# Z # Z  �� "  ��6�  �� *CellsPanelHorizontalOffset+ �� "    5-   .   $! PART_ColumnHeadersPresenter��6M   $��1��6r   y�6�   �� *HeadersVisibility+# Z # Z  �� "  �� 5.   .   $! PART_ScrollContentPresenter��6E   # z��6j   $��2��6�   $��1��6�   �� 5/   .   $ PART_VerticalScrollBar��68   $��2��6X   # z 6h   $ Vertical=�6�   $��1��6�   #y�z 6�   # z 6[   6�   �� *VerticalOffset+$  OneWay=� �� *TemplatedParent+6[  � 50   .   $��1��63   $��2��6C   	 51   2   �� 52   6   ��6G   �� **(NonFrozenColumnsViewportHorizontalOffset+ �� "  �� 53   6   $��* �6G   .+�55   2   �� $ PART_HorizontalScrollBar��6<   $��1��6^   # z! 6n   $ 
Horizontal=�6�   #y�z" 6�   # z# 6Y   6�   �� *HorizontalOffset+$  OneWay=� �� *TemplatedParent+6Y  56   /   57   +   58   '   .��5:   "   �� # z 61   5;      5<      5=      5?      $ 5@      ]� 5A      % 5B      �� 5C      & �@��
IsGrouping' . 6:   ./�5F      ��  � �� .  6F   5G      5I      5J      .��5K      � $��	10,10,0,0q�6   .+�5L   
     -   5        (  Name$( GRPFList��5L      $
��Left=�6>   $	��361��6Y   $��	0,116,0,0q�6f   $	��Top=�6y   )   AutoGenerateColumns) .  $��Center=�6�   "*   VerticalScrollBarVisibility$* Visible=�6�   $��14(�6�   +   
IsReadOnly+ . ���	#FF1654AE#���  6.  $��Center=�6W  ��6&   �� ,   ColumnHeaderStyle, 5M      �� 5N      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  5O      .�5P      ��  � ��$ Center=�5Q   ?   5R      -   Columns- 5W      0  *System.Windows.Controls.DataGridTextColumn  5X      ,  &System.Windows.Controls.DataGridColumn.   Header$. 预报员ID��6%   /   Width5  /System.Windows.Controls.DataGridLengthConverter$/ 90 6R   1  +System.Windows.Controls.DataGridBoundColumn0   ElementStyle#0 [  6]   1   FontSize$1 16Q�6�   2   Binding2 64   �� *
PeopleID+6�     5Y      $. 姓名��6%   $/ 70 6Q   #0 [  6\   $1 16Q�6�   2 61   �� *
PeopleName+6�     5Z      $. 排名��6%   $/ 50 6I   #0 [  6T   $1 16Q�6�   2 61   �� *PM+6�     5[      $. 值班次数��6%   $/ 62 6M   #0 [  6X   $1 16Q�6�   2 63   �� *ZBCS+6�     5\      $. 值班基数��6%   $/ 62 6M   #0 [  6X   $1 16Q�6�   2 63   �� *ZBJS+6�     5]      $. 晴雨准确率��6%   $1 18Q�6M   $/ 75 6[   #0 [  6f   3   SortMemberPath$3  ��6�   2 64   �� *QYPF+6�     5^      $. 高温准确率��6%   $1 18Q�6M   $/ 75 6[   #0 [  6f   2 64   �� *GWPF+6f     5_      $. 低温准确率��6%   $1 18Q�6M   $/ 75 6[   #0 [  6f   2 64   �� *DWPF+6f     5`      $. 平均准确率��6%   $1 18Q�6M   $/ 75 6[   #0 [  6f   2 64   �� *ZHPF+6f     5a      $. 晴雨技巧��6%   $1 18Q�6L   $/ 65 6Z   #0 [  6e   2 63   �� *QYJQ+6e     5b      $. 高温技巧��6%   $1 18Q�6L   $/ 65 6Z   #0 [  6e   2 63   �� *GWJQ+6e     5c      $. 低温技巧��6%   $1 18Q�6L   $/ 65 6Z   #0 [  6e   2 63   �� *DWJQ+6e     5d      $. 技巧总评分��6%   $1 18Q�6N   $/ 75 6\   #0 [  6g   2 64   �� *AllJQ+6g   5g      (	  "System.Windows.Controls.DatePicker	  5h   
   -   5        4 	 Name$4 sDate��5h      $
��Left=�6%   $��
227,42,0,0q�6@   $	��Top=�6T   $��28��6l   $	��130��6x   
���   �
�������5 	  IsTodayHighlighted5 . 6 	  SelectedDateFormat$
6 Long=�6�   6�   	  5i   
   -   5        $4 eDate��5i      $
��Left=�6$   $��
442,42,0,0q�6?   $	��Top=�6S   $��28��6k   $	��130��6w   
���   �
�������5 .  $
6 Long=�6�   �� 5j   
   -   5        7 ��Name$7 labelStartdate��5j      $��开始日期��6(   $
��Left=�67   $��
258,11,0,0q�6R   $	��Top=�6f   $��26��6~   $��58��6�   8 J� RenderTransformOrigin$8 
-2.466,0.5&�6�   �� 5k   
   -   5        $7 labelEnddate��5k      $��结束日期��6&   $
��Left=�65   $��
473,11,0,0q�6P   $	��Top=�6d   $8 0.362,3&�6|   $��26��6�   $��58��6�   �� 5l   
   -   5        $7 BTLabel��5l      $�� ��6!   $
��Left=�6,   $��
141,78,0,0q�6G   $	��Top=�6[   $��Center=�6s   $��Center=�6�   $	��659��6�   $��20(�6�   $��楷体)�6�   �� 5m   
   -   5        9 ��Name$9 CXButton��5m      $��查询��6#   $
��Left=�60   $��
646,42,0,0q�6K   $	��Top=�6_   $	��110��6w   $��27��6�   ���	#FF5BABEE
�������6�   �� 5o   
   -   5        $9 DCButton��5o      $��导出��6#   $
��Left=�60   $��
827,42,0,0q�6K   $	��Top=�6_   $	��110��6w   $��27��6�   ���	#FF5BABEE
�������6�   �� 5p   
   -	   5        : ��Name$: SCSelect��5p      ; �� SelectedIndex$; 0��6%   $
��Left=�67   $��	48,42,0,0q�6R   $	��Top=�6e   $	��110��6}   ���	#FF5BABEE$��Center=�6�   $��Center=�6�   $��27��6�   $��16(�6�   $��ExtraExpanded&�6   .@�5q      �� $��综合��6   $	��100��6(   $	��100��65   5r      �� 5s   
   $��时次��6   $
��Left=�6   $��	79,11,0,0q�68   $	��Top=�6K   5u      5v      