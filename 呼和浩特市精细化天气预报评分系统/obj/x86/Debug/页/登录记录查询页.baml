   M S B A M L   `   `   `  ����k  g呼和浩特市精细化天气预报评分系统, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulls>clr-namespace:呼和浩特市精细化天气预报评分系统0呼和浩特市精细化天气预报评分系统  5      L    F呼和浩特市精细化天气预报评分系统.登录记录查询页   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Plocal>clr-namespace:呼和浩特市精细化天气预报评分系统     5        N� Title$  登录记录查询页��5	      $	��528��6    � 	Resources 5         contentCenterStyle �� 
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
IsGrouping' . 6:   ./�5E      ��  � �� .  6F   5F      5H      5I      .��5J      � $��0,0,0,0q�6   .+�5K   
     -   5        (  Name$( ZBList��5K      $
��Left=�6<   $	��405��6W   $��
31,135,0,0q�6d   $	��Top=�6x   )   AutoGenerateColumns) .  $��Center=�6�   "*   VerticalScrollBarVisibility$* Visible=�6�   $��16(�6�   +   
IsReadOnly+ . ���	#FF1654AE#���  6-  $��Center=�6V  ��6$   �� ,   ColumnHeaderStyle, 5L      �� 5M      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  5N      .�5O      ��  � ��$ Center=�5P   ?   5Q      -   Columns- 5V      0  *System.Windows.Controls.DataGridTextColumn  5W      ,  &System.Windows.Controls.DataGridColumn.   Header$. 日期��6%   /   Width5  /System.Windows.Controls.DataGridLengthConverter$	/ 160 6H   1  +System.Windows.Controls.DataGridBoundColumn0   ElementStyle#0 [  6T   1   FontSize$1 18Q�6�   2   Binding2 61   �� *RQ+6�     5X      $. 领班��6%   $1 18Q�6H   $	/ 100 6V   #0 [  6b   2 61   �� *LB+6b     5Y      $. 主班08��6%   $1 18Q�6L   $	/ 100 6Z   #0 [  6f   2 63   �� *ZB08+6f     5Z      $. 主班20��6%   $1 18Q�6L   $	/ 100 6Z   #0 [  6f   2 63   �� *ZB20+6f   5]      �� 5^   
   -   5        3 ��Name$3 monthchoose��5^      $
��Left=�6)   $��
133,50,0,0q�6D   $	��Top=�6X   $��60��6p   $��28��6{   $��16(�6�   $��Center=�6�   $��Center=�6�   4 � ToolTip$4 请选择月��6�   .@�5_      �� $��01��6   $��25��6(   $��60��64   �� 5`      $��02��6   $��25��6&   $��60��62   �� 5a      $��03��6   $��25��6&   $��60��62   �� 5b      $��04��6   $��25��6&   $��60��62   �� 5c      $��05��6   $��25��6&   $��60��62   �� 5d      $��06��6   $��25��6&   $��60��62   �� 5e      $��07��6   $��25��6&   $��60��62   �� 5f      $��08��6   $��25��6&   $��60��62   �� 5g      $��09��6   $��25��6&   $��60��62   �� 5h      $��10��6   $��25��6&   $��60��62   �� 5i      $��11��6   $��25��6&   $��60��62   �� 5j      $��12��6   $��25��6&   $��60��62   5k      �� 5l   
   -   5        5 ��Name$5 BTLabel��5l      $�� ��6!   $
��Left=�6,   $��	95,89,0,0q�6G   $	��Top=�6Z   $��Center=�6r   $��Center=�6�   $	��343��6�   $��20(�6�   $��楷体)�6�   �� 5m   
   -   5        6 ��Name$6 CXButton��5m      $��查询��6#   $
��Left=�60   $��
222,51,0,0q�6K   $	��Top=�6_   $	��110��6w   $��27��6�   ���	#FF5BABEE
�������6�   �� 5n   
   -   5        $6 DCButton��5n      $��导出��6#   $
��Left=�60   $��
370,50,0,0q�6K   $	��Top=�6_   $	��110��6w   $��27��6�   ���	#FF5BABEE
�������6�   �� 5o   
   -   5        $3 
yearchoose��5o      $
��Left=�6)   $��	25,50,0,0q�6D   $	��Top=�6W   $��85��6o   $��28��6z   $��16(�6�   $��Center=�6�   $��Center=�6�   $4 请选择年��6�   �� 5p   
   $	��年��6   $
��Left=�6   $��	50,20,0,0q�67   $	��Top=�6J   $��16(�6b   �� 5q   
   $	��月��6   $
��Left=�6   $��
150,20,0,0q�67   $	��Top=�6K   $��16(�6c   5s      5t      