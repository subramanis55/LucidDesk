   M S B A M L   `   `   `  ����D  @LucidDesk, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:LucidDesk	LucidDesk  5   	   M+clr-namespace:LucidDesk.UserControls.CommonLucidDesk.UserControls.Common  5   
       LucidDesk.InviteWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:LucidDesk     5   	   CUsercontrol+clr-namespace:LucidDesk.UserControls.Common     5   
     � 
ResizeMode$  NoResize=�5       � WindowStartupLocation$ CenterScreen=�64    � Title$ InviteWindow��5	   	   $	��350��6   $	��600��6+    � WindowStyle$
 None=�67   
�������$��1q�6c      SubColorBrush#��[  6w   .��5      �  � RowDefinitions 5   
   �� 5      $��40 �6   �� 5      $��* �6   .+�5   
   ]� -   5        #��[  5      .+�5      ��  �� Padding$ 20 10q�6   $��17(�6(     MainColorBrush#��[ 66   $��SemiBold"�6c   Invite6�   �� 5      -   5        $��Right=�5        CloseButton#��[ 61   $��35��6V   $��35��6b    � ToolTip 5      _� 5      $��Close��6   60   .��5      F� $��Stretch=�6   $��12��63   $��12��6?   $��Stretch=�6K   
�������$	��1.5��6w   +����        � �  � �     � � �     	6�   5      5      �� 5   
   $��1��6   .+�5      �� $��2q�6   .+�5      �� $��14(�6   $��Medium"�6*   $��10q�6>   	User Id6S   � 5      .+�5      1   +LucidDesk.UserControls.Common.CustomTextBox  -   5          Name$ Textbox��5   0   1   +LucidDesk.UserControls.Common.CustomTextBox   GotFocus$ TextboxGotFocus��6O   	   	LostFocus$	 TextboxLostFocus��6j   
   TextChanged$
 TextboxTextChanged��6�   $��15(�6A   $��Medium"�6�   
�������$��1q�6�   
�������$��40��6�   $	��350��6  6  �� 5      -   5        $��UserIdLabelContainer��5       K� ZIndex$ -1��69   
�������$��1q�6_    �� CornerRadius$ 2��6t   $��40��6�   $	��350��6�   
�������._�5       �� -   5        $��UserIdLabelTextblock��5    $   $��Center=�6B   $ 12q�6]   $��15(�6k   $��Medium"�6z   6�   5!      5"      5$      �� 5%      $��2q�6   .+�5&      �� $��14(�6   $��Medium"�6*   $��10q�6>   Profile Access6Z   �� 5'      -   5        $��AccessTypeCombobox��5'        CustomComboBox#��[ 6l   
�������
�������$��1q�6�   $	��350��6�   $��40��6�   5)      5*      �� 5+      ._�5,      �� $��8q�6   $��Center=�6(    �� Orientation$ 
Horizontal=�6E   .+�5-      �� -   5        $��KeyboardAccessCheckBox��5-      $��Center=�6>     AccessIconCheckBox#��[ 6[   $��3q�6�   $��5q�6�   
�������$��1q�6�   $��45��6�   $��45��6�    5.      _� 5/      $��Keyboard access��6&   6B   .��51      (� .��52      F�   Keyboard#��[ 6z   ��6#   ��  �� RelativeSource ��  �� AncestorType$ CheckBoxL� �� Path$ 
Foreground�6�   53      54      �� 55      -   5        $��MouseAccessCheckBox��55      $��Center=�6:   #��[ 6W   $��3q�6�   $��5q�6�   
�������$��1q�6�   $��45��6�   $��45��6�    56      _� 57      $��Mouse access��6&   6?   .��59      (� $��0 0 0 4q�6"   .��5:      F�  	 Mouse#��[ 6z   ��6#   ��  �� $ CheckBoxL�$ 
Foreground�6z   5;      5<      �� 5=      -	   5        $��ClipboardAccessCheckBox��5=       #��[ 6A   $��3q�6m   $��5q�6z   
�������$��1q�6�   $��45��6�   $��45��6�    5>      _� 5?      $��Clipboard access��6&   6C   .��5A      (� $��0 0 4 4q�6#   $��Center=�64   $��Center=�6O   .��5B      F�   	ClipBoard#��[ 6}   ��6#   ��  �� $ CheckBoxL�$ 
Foreground�6}   5C      5D      �� 5E      -
   5        $��AudioAccessCheckBox��5E      #��[ 6;   $��3q�6g   $��5q�6t   
�������$��1q�6�   $��45��6�   $��45��6�    5F      _� 5G      $��Audio access��6&   6?   .��5I      (� $��0 0 4 4q�6#   $��Center=�64   $��Center=�6O   .��5J      F�   Speaker#��[ 6}   ��6#   ��  �� $ CheckBoxL�$ 
Foreground�6}   5K      5L      5M      5N      �� 5O      $��10q�6   ._�5P      �� -   5        $��InviteButton��5P      $��40��6G    	 NormalButton#��[	 6S   $��15(�6z   $��Invite��6�   
�������#��[ 6�   $	��120��6�   6�   5Q      5R      5T      5U      